namespace DigitalMarketplace.Core.DTOs.Users;
public record GetUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    string ImageUri);