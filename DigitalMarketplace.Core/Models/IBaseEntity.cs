namespace DigitalMarketplace.Core.Models;
public interface IBaseEntity
{
    public Guid Id { get; }
    public DateTime? CreatedAt { get; }
    public DateTime? UpdatedAt { get; }
}
