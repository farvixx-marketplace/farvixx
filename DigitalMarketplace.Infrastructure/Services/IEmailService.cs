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
    Task<ServiceResponse<MessageResult>> SendEmailAsync(string email, string content, string subject);
    Task<ServiceResponse<IEnumerable<MessageResult>>> SendEmailAsync(IEnumerable<string> email, string content, string subject);
}
