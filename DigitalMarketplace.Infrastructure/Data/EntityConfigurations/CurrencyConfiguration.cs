using DigitalMarketplace.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(c => c.Code);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.CurrencySymbol)
            .HasMaxLength(10);
    }
}
