using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.ValueObjects;
using DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketplace.Infrastructure.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Currency> Currency => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
