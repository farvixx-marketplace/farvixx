using DigitalMarketplace.Core.Enums;
using DigitalMarketplace.Core.Models;

namespace DigitalMarketplace.Core.DTOs.Products;
public record GetProductFullDto(
    Guid Id,
    string Title,
    string Description,
    
    decimal Price,
    Currency Currency,
    Category Category,
    string[] Tags,
    AdStatus AdStatus);