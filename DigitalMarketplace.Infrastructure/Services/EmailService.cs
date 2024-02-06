using DigitalMarketplace.Core.DTOs;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client.TransactionalEmails.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMarketplace.Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly IMailjetClient _client;
    private readonly TransactionalEmailBuilder _emailBuilder;

    TransactionalEmailBuilder EmailBuilder { get => _emailBuilder.Clone(); }

    public EmailService(IMailjetClient mailjetClient)
    {
        _client = mailjetClient;

        _emailBuilder = new TransactionalEmailBuilder();
        _emailBuilder.WithFrom(new ("m.rusnachenko@outlook.com", "Farvix"));
    }

    public async Task<ServiceResponse<MessageResult>> SendEmailAsync(string toEmail, string content, string subject)
    {
        var serviceResponse = new ServiceResponse<MessageResult>();

        var email = EmailBuilder.WithSubject(string.IsNullOrWhiteSpace(subject) ? "Test subject" : subject)
               .WithHtmlPart(string.IsNullOrWhiteSpace(content) ? "<h1>Header</h1>" : content)
               .WithTo(new SendContact(toEmail))
               .Build();

        // invoke API to send email
        var emailsResponse = await _client.SendTransactionalEmailAsync(email);

        // check response
        if (emailsResponse.Messages.Length < 0)
        {
            return serviceResponse.Failed(null, $"Errors: {emailsResponse.Messages.Select(m => string.Join(", ", m.Errors.Select(e => e.ErrorMessage)))}");
        }

        return serviceResponse.Succeed(emailsResponse.Messages[0]);
    }

    public async Task<ServiceResponse<IEnumerable<MessageResult>>> SendEmailAsync(IEnumerable<string> toEmails, string content, string subject)
    {
        var serviceResponse = new ServiceResponse<IEnumerable<MessageResult>>();

        var builder = EmailBuilder.WithSubject(subject)
            .WithHtmlPart(content);
        List<TransactionalEmail> emails = [];
        foreach (var toEmail in toEmails)
        {
            builder.WithTo(new SendContact(toEmail));
            emails.Add(builder.Build());
        }

        // invoke API to send email
        var emailsResponse = await _client.SendTransactionalEmailsAsync(emails);

        // check response
        if (emailsResponse.Messages.Length < 0)
        {
            return serviceResponse.Failed(null, $"Errors: {emailsResponse.Messages.Select(m => string.Join(", ", m.Errors.Select(e => e.ErrorMessage)))}");
        }

        return serviceResponse.Succeed(emailsResponse.Messages);
    }
}
