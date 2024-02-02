using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.DTOs.Users;
public record UpdateUserDto(
    string? ImageUri = null,
    string? FirstName = null,
    string? LastName = null,
    string? Country = null,
    string? City = null,
    string? Gender = null,
    DateTime? BirthDate = null,
    decimal? Balance = null,
    string? CurrencyCode = null,
    List<Language>? Languages = null,
    List<Tag>? Tags = null,
    List<Guid>? CategoryIds = null,
    List<ExternalResource>? ExternalResources = null,
    string? Bio = null
);
