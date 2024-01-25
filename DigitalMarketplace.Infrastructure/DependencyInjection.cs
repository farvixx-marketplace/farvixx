using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using DigitalMarketplace.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalMarketplace.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => 
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(true);
        });

        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
