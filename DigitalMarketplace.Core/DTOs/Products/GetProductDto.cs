using DigitalMarketplace.Core.Enums;
using DigitalMarketplace.Core.Models;

namespace DigitalMarketplace.Core.DTOs.Products;
public record GetProductDto(
    Guid Id,
    string Title,
    decimal Price,
    Currency Currency,
    Category Category,
    AdStatus AdStatus);