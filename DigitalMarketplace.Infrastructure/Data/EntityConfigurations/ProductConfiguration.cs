﻿using DigitalMarketplace.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalMarketplace.Infrastructure.Data.EntityConfigurations;
internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(p => p.Category);

        builder.HasMany(p => p.Tags);

        builder.Property(p => p.Price)
            .IsRequired();

        builder.ComplexProperty(p => p.Currency)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UpdatedAt)
            .ValueGeneratedOnUpdate();
    }
}
