using BunnyCDN.Net.Storage;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using DigitalMarketplace.Infrastructure.Services;
using Mailjet.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalMarketplace.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, string mailjetApiKey, string mailjetApiSecret, string bunnyCDNKey)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(true);
        });

        services.AddScoped(client => new BunnyCDNStorage("main-storage-45", bunnyCDNKey));

        services.AddScoped<IMailjetClient, MailjetClient>(client =>
        {
            return new MailjetClient(mailjetApiKey, mailjetApiSecret);
        });
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
