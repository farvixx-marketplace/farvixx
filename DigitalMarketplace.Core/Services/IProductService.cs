using DigitalMarketplace.Core.Models;

namespace DigitalMarketplace.Core.Services;
public interface IProductService
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product> GetProductById(int productId);
    Task<int> AddProduct(Product product);
    Task<int> DeleteProduct(int productId);
}
