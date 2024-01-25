using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.Models;
public class User : BaseEntity
{
    public string ImageUri { get; set; } = string.Empty;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public Location? Location { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public DateTime? BirthDate { get; set; }

    public decimal Balance { get; set; }
    public Currency Currency { get; set; } = new Currency { Name = "United States Dollar", Code = "USD", CurrencySymbol = "$", HtmlCode = "&#36;" };

    public List<Language> Languages { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<ExternalResource> ExternalResources { get; set; } = [];
    public string Bio { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];

    public static MinimalUser GetMinimal(User user)
    {
        return new MinimalUser(user.Id,
                               user.ImageUri,
                               user.FirstName,
                               user.LastName,
                               user.Username,
                               user.Email);
    }

    public User Update(UpdateUserDto updateUser)
    {
        ImageUri = updateUser.ImageUri ?? ImageUri;
        FirstName = updateUser.FirstName ?? FirstName;
        LastName = updateUser.LastName ?? LastName;
        Username = updateUser.Username ?? Username;
        Email = updateUser.Email ?? Email;

        if (Location is null)
        {
            if (updateUser.Country is not null)
            {
                Location = new Location
                {
                    Country = updateUser.Country,
                    City = updateUser.City ?? Location!.City
                };
            }
        }
        else
        {
            Location.Country = updateUser.Country ?? Location.Country;
            Location.City = updateUser.City ?? Location.City;
        }

        Gender = updateUser.Gender ?? Gender;
        Phone = updateUser.Phone ?? Phone;
        BirthDate = updateUser.BirthDate ?? BirthDate;
        Bio = updateUser.Bio ?? Bio;

        Languages = updateUser.Languages ?? Languages;
        ExternalResources = updateUser.ExternalResources ?? ExternalResources;

        return this;
    }
}
