namespace DigitalMarketplace.Core.Models;
public record MinimalUser(Guid Id,
                          string ImageUri,
                          string FirstName,
                          string LastName,
                          string? Email,
                          string? Username);
