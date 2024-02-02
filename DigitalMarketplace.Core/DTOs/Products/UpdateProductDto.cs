namespace DigitalMarketplace.Core.DTOs.Products;
public record UpdateProductDto(
    string? Title,
    string? Description,
    decimal? Price,
    string? Currency,
    string? Category,
    string[]? Tags);