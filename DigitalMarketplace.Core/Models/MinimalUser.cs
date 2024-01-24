namespace DigitalMarketplace.Core.Models;
public record MinimalUser(int? Id,
                          string ImageUri,
                          string FirstName,
                          string LastName,
                          string Username,
                          string Email);
