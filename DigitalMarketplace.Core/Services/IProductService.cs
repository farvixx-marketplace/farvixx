using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Enums;

namespace DigitalMarketplace.Core.Services;
public interface IProductService
{
    Task<ServiceResponse<IEnumerable<GetProductDto>>> GetProducts();
    Task<ServiceResponse<GetProductFullDto>> GetProductById(Guid productId);
    Task<ServiceResponse<Guid?>> AddProduct(Guid userId, AddProductDto product);
    Task<ServiceResponse<Guid?>> AddProduct(Guid userId, AddProductDto product, Stream content);
    Task<ServiceResponse<bool>> DeleteProduct(Guid userId, Guid productId);
    Task<ServiceResponse<Guid?>> UpdateProduct(Guid productId, UpdateProductDto updateProduct);
    Task<ServiceResponse<bool>> ChangeProductStatus(Guid productId, AdStatus status);
    Task<ServiceResponse<IEnumerable<GetProductDto>>> GetUsersProducts(Guid userId);
    Task<ServiceResponse<IEnumerable<GetProductDto>>> GetSimilarProducts(Guid productId);
    Task<ServiceResponse<Guid?>> UpdateProductContent(Guid userId, Guid productId, Stream content);
    Task<ServiceResponse<string?>> UploadProductContent(Stream fileStream, string fileName);
    Task<ServiceResponse<Stream>> DownloadProductContent(string fileName);
    Task<ServiceResponse<Stream>> DownloadProductContent(Guid productId);
    Task DeleteProductContent(string fileName);
}
