using DigitalMarketplace.Core.Enums;

namespace DigitalMarketplace.Core.DTOs.Products;
public record AddProductDto(string Title,
    Guid CategoryId,
    string[] Tags,
    string Description,
    decimal Price,
    string Currency = "USD",
    string? Content = null,
    AdStatus AdStatus = AdStatus.Hidden);
