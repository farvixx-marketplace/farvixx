using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketplace.Core.Models;
public class User : IdentityUser<Guid>, IBaseEntity
{
    public string ImageUri { get; set; } = string.Empty;
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Location Location { get; set; } = new Location { Country = "United States of America", Alpha2Code = "US" };
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }

    public decimal Balance { get; set; }
    public Currency Currency { get; set; } = new Currency { Name = "United States Dollar", Code = "USD", CurrencySymbol = "$", HtmlCode = "&#36;" };

    public List<Language> Languages { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<ExternalResource> ExternalResources { get; set; } = [];
    public string Bio { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    public static GetUserDto GetUserDto(User user)
    {
        return new GetUserDto(user.Id,
                               user.ImageUri,
                               user.FirstName,
                               user.LastName,
                               user.Email,
                               user.UserName);
    }

    public User Update(UpdateUserDto updateUser)
    {
        ImageUri = updateUser.ImageUri ?? ImageUri;
        FirstName = updateUser.FirstName ?? FirstName;
        LastName = updateUser.LastName ?? LastName;

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
        BirthDate = updateUser.BirthDate ?? BirthDate;
        Bio = updateUser.Bio ?? Bio;

        Languages = updateUser.Languages ?? Languages;
        ExternalResources = updateUser.ExternalResources ?? ExternalResources;

        return this;
    }
}
