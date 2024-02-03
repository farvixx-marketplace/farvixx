using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.DTOs.Users;
public record GetUserFullDto(
    Guid Id,
    string? ImageUri,
    string FirstName,
    string LastName,
    string? UserName,
    string? Email,
    string? PhoneNumber,
    Location? Location,
    decimal Balance = 0,
    Currency? Currency = null,
    string? Gender = null,
    DateTime? BirthDate = null,
    IEnumerable<Language>? Languages = null,
    IEnumerable<Tag>? Tags = null,
    IEnumerable<Category>? CategoryIds = null,
    IEnumerable<ExternalResource>? ExternalResources = null,
    string? Bio = null
    );
