using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Enums;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketplace.Infrastructure.Services;
public class ProductService(ApplicationDbContext dbContext) : IProductService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<ServiceResponse<Guid>> AddProduct(Guid userId, AddProductDto newProduct)
    {
        var serviceResponse = new ServiceResponse<Guid>();

        var productTags = new List<Tag>();
        foreach (var tagString in newProduct.Tags)
        {
            var tag = await _dbContext.Tags.FindAsync(tagString);
            
            tag ??= (await _dbContext.Tags.AddAsync(new Tag { Name = tagString })).Entity;
            productTags.Add(tag);
        }

        var product = new Product
        {
            Title = newProduct.Title,
            Description = newProduct.Description,
            Currency = (await _dbContext.Currency.FindAsync(newProduct.Currency))
                       ?? (await _dbContext.Currency.FindAsync("USD"))!,
            Price = newProduct.Price,
            Category = await _dbContext.Categories.FindAsync(newProduct.CategoryId)
                       ?? (await _dbContext.Categories.FindAsync("Other"))!,
            Tags = productTags,
            AdStatus = newProduct.AdStatus,
        };

        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return serviceResponse.Succeed(product.Id);
        }

        return serviceResponse.Failed(Guid.Empty, "");
    }

    public Task<ServiceResponse<bool>> ChangeProductStatus(Guid productId, AdStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<Guid>> DeleteProduct(Guid productId)
    {
        throw new NotImplementedException();
    }


    public async Task<ServiceResponse<GetProductFullDto>> GetProductById(Guid productId)
    {
        var serviceResponse = new ServiceResponse<GetProductFullDto>();

        var product = await _dbContext.Products.Include(p => p.Currency)
                                               .Include(p => p.Owner)
                                               .Include(p => p.Category)
                                               .Include(p => p.Tags)
                                               .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is not null)
            product.Owner.Products = [];

        if (product is null)
            return serviceResponse.Failed(null, "Could not find product with given Id");

        var productDto = new GetProductFullDto(Id: product.Id, product.Title, product.Description, product.Price, product.Currency, product.Category, product.Tags.Select(t => t.Name).ToArray(), product.AdStatus);

        return serviceResponse.Succeed(productDto);
    }

    public async Task<ServiceResponse<IEnumerable<GetProductDto>>> GetProducts()
    {
        var serviceResponse = new ServiceResponse<IEnumerable<GetProductDto>>();

        var products = (await _dbContext.Products.Take(50).ToArrayAsync()).Select(p => new GetProductDto(
            Id: p.Id,
            Title: p.Title,
            Currency: p.Currency,
            Price: p.Price,
            Category: p.Category,
            AdStatus: p.AdStatus));
        return serviceResponse.Succeed(products);
    }

    public Task<ServiceResponse<IEnumerable<GetProductDto>>> GetSimilarProducts(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<IEnumerable<GetProductDto>>> GetUsersProducts(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<Guid>> UpdateProduct(Guid productId, UpdateProductDto updateProduct)
    {
        throw new NotImplementedException();
    }
}
