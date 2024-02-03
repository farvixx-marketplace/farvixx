using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DigitalMarketplace.Core.ValueObjects;
[Owned]
public class Location
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Country { get; set; }
    public string Alpha2Code { get; set; }
    public string City { get; set; }
}

