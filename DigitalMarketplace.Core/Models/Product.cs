using DigitalMarketplace.Core.Enums;
using DigitalMarketplace.Core.ValueObjects;

namespace DigitalMarketplace.Core.Models;
public class Product : BaseEntity
{
    public User Owner { get; set; }
    public string Title { get; set; }
    
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public string Description { get; set; }
    public AdStatus AdStatus { get; set; } = AdStatus.Hidden;
}
