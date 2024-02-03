using DigitalMarketplace.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(25)
            .IsRequired();

        builder.OwnsOne(u => u.Location)
            .WithOwner();

        builder.OwnsOne(u => u.Location)
            .Property(l => l.Country)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(u => u.Location)
            .Property(l => l.Alpha2Code)
            .IsFixedLength()
            .HasMaxLength(2)
            .IsRequired();

        builder.OwnsOne(u => u.Location)
            .Property(l => l.City)
            .HasMaxLength(50);
        //builder.Ignore(u => u.Location);

        builder.Property(u => u.Gender)
            .HasMaxLength(50);

        builder.OwnsMany(u => u.Languages)
            .WithOwner();

        builder.HasMany(u => u.Tags);

        builder.HasMany(u => u.Categories);

        builder.OwnsMany(u => u.ExternalResources)
            .WithOwner();

        builder.Property(u => u.Bio)
            .HasColumnType("text");

        builder.HasMany(u => u.Products)
            .WithOne(p => p.Owner);

        builder.Property(u => u.Balance);

        builder.HasOne(u => u.Currency);

        builder.Property(u => u.CreatedAt);

        builder.Property(u => u.UpdatedAt);
    }
}
