using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.DTOs.Users;
public record GetUserFullDto(
    Guid Id,
    string ImageUri,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string? PhoneNumber,
    Location? Location,
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
