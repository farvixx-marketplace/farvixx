using DigitalMarketplace.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasKey(c => c.Name);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(c => c.CurrencySymbol)
            .HasMaxLength(10);

        builder.Property(c => c.HtmlCode)
            .HasMaxLength(10);
    }
}
