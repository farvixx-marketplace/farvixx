using DigitalMarketplace.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketplace.Core.Models;
public class Product : IBaseEntity
{
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public Guid OwnerId { get; set; }

    public string Title { get; set; }
    
    public decimal Price { get; set; }
    public Currency Currency { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public string Description { get; set; }
    public AdStatus AdStatus { get; set; } = AdStatus.Hidden;

    public string? Content { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
