using DigitalMarketplace.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasMaxLength(55)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.OwnsOne(u => u.Location)
            .WithOwner();

        builder.Property(u => u.Gender)
            .HasMaxLength(50);

        builder.Property(u => u.Phone)
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

        builder.Property(u => u.CreatedAt)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.UpdatedAt)
            .ValueGeneratedOnUpdate();
    }
}
