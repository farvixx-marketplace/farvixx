using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketplace.Core.Models;
public class BaseEntity
{
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}
