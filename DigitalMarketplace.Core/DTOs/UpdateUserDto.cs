using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.DTOs;
public record UpdateUserDto(
    int Id,
    string? ImageUri = null,
    string? FirstName = null,
    string? LastName = null,
    string? Username = null,
    string? Email = null,
    string? Country = null,
    string? City = null,
    string? Gender = null,
    string? Phone = null,
    DateTime? BirthDate = null,
    decimal? Balance = null,
    string? CurrencyCode = null,
    List<Language>? Languages = null,
    List<Tag>? Tags = null,
    List<int>? CategoryIds = null,
    List<ExternalResource>? ExternalResources = null,
    string? Bio = null
);
