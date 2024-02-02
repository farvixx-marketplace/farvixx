namespace DigitalMarketplace.Core.DTOs.Users;
public record AddUserDto(
    string FirstName,
    string LastName,
    string Email,
    string? ImageUri = null);