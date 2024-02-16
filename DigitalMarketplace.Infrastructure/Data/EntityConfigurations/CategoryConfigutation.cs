using DigitalMarketplace.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class CategoryConfigutation : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children);

        builder.HasMany(c => c.Children)
            .WithOne(c => c.Parent);

        builder.HasMany(c => c.Users)
            .WithMany(u => u.Categories);

        builder.HasMany(c => c.Products)
            .WithOne(c => c.Category);
    }
}
