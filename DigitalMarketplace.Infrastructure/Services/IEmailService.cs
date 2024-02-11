using DigitalMarketplace.Core.DTOs;
using Mailjet.Client.TransactionalEmails.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMarketplace.Infrastructure.Services;
public interface IEmailService
{
    Task<ServiceResponse<MessageResult>> SendEmailAsync(string email, string content, string subject, IDictionary<string, object>? variables = default);
    Task<ServiceResponse<MessageResult>> SendEmailConfirmationLetter(string email, string token);
    Task<ServiceResponse<IEnumerable<MessageResult>>> SendEmailsAsync(IEnumerable<string> email, string content, string subject, IEnumerable<IDictionary<string, object>>? variables = default);
}
