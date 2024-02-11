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

    public async Task<ServiceResponse<MessageResult>> SendEmailAsync(string toEmail, string content, string subject, IDictionary<string, object>? variables = default)
    {
        var serviceResponse = new ServiceResponse<MessageResult>();

        var email = EmailBuilder.WithSubject(string.IsNullOrWhiteSpace(subject) ? "Test subject" : subject)
               //.WithHtmlPart(content)
               .WithTemplateId(5684263)
               .WithTo(new SendContact(toEmail))
               .WithVariables(variables)
               .WithTemplateLanguage(true)
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

    public async Task<ServiceResponse<IEnumerable<MessageResult>>> SendEmailsAsync(IEnumerable<string> toEmails, string content, string subject, IEnumerable<IDictionary<string, object>>? variables = default)
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

    public async Task<ServiceResponse<MessageResult>> SendEmailConfirmationLetter(string toEmail, string token)
    {
        var serviceResponse = new ServiceResponse<MessageResult>();

        var email = EmailBuilder.WithSubject("Confirm your email address")
               //.WithHtmlPart(content)
               .WithTemplateId(5684263)
               .WithTo(new SendContact(toEmail))
               .WithVariables(new Dictionary<string, object>{ { "token", token } })
               .WithTemplateLanguage(true)
               .Build();

        // invoke API to send email
        var emailsResponse = await _client.SendTransactionalEmailAsync(email);

        // check response
        if (emailsResponse.Messages.Length < 0 || emailsResponse.Messages[0].Status == "error")
        {
            return serviceResponse.Failed(null, $"Errors: {emailsResponse.Messages.Select(m => string.Join(", ", m.Errors.Select(e => e.ErrorMessage)))}");
        }

        return serviceResponse.Succeed(emailsResponse.Messages[0]);
    }
}
