using BunnyCDN.Net.Storage;
using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Enums;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketplace.Infrastructure.Services;
public class ProductService(
    ApplicationDbContext dbContext,
    BunnyCDNStorage bunnyCDNStorage) : IProductService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly BunnyCDNStorage _storage = bunnyCDNStorage;

    public async Task<ServiceResponse<Guid?>> AddProduct(Guid userId, AddProductDto newProduct)
    {
        var serviceResponse = new ServiceResponse<Guid?>();

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
                       ?? (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Other"))
                       ?? (await _dbContext.Categories.AddAsync(new Category { Name = "Other"})).Entity,
            Tags = productTags,
            AdStatus = newProduct.AdStatus,
            Content = newProduct.Content!
        };

        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return serviceResponse.Failed(null, "");
        }

        user.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return serviceResponse.Succeed(product.Id);
        
    }

    public async Task<ServiceResponse<Guid?>> AddProduct(Guid userId, AddProductDto newProduct, Stream content)
    {
        var response = new ServiceResponse<Guid?>();

        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
            return response.Failed(null, "User does not exist");

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
                       ?? (await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Other"))
                       ?? (await _dbContext.Categories.AddAsync(new Category { Name = "Other" })).Entity,
            Tags = productTags,
            AdStatus = newProduct.AdStatus,
            Content = newProduct.Content!
        };

        var contentUpload = await UploadProductContent(content, product.Title);
        if (!contentUpload.Success || string.IsNullOrWhiteSpace(contentUpload.Data))
            return response.Failed(null, $"Could not upload product content. {contentUpload.Error}");

        product.Content = contentUpload.Data;

        user.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return response.Succeed(product.Id);
    }

    public async Task<ServiceResponse<bool>> ChangeProductStatus(Guid productId, AdStatus status)
    {
        var response = new ServiceResponse<bool>();

        var product = await _dbContext.Products.FindAsync(productId);
        if (product is null)
        {
            return response.Failed(false, "Product with given ID could not be found");
        }

        if (status == AdStatus.Public && string.IsNullOrWhiteSpace(product.Content))
        {
            return response.Failed(false, "Product is not eligible to be public until the contents are uploaded");
        }

        product.AdStatus = status;

        await _dbContext.SaveChangesAsync();
        return response.Succeed(true);
    }

    public async Task<ServiceResponse<bool>> DeleteProduct(Guid userId, Guid productId)
    {
        var response = new ServiceResponse<bool>();

        var product = await _dbContext.Products.FirstOrDefaultAsync(p => productId.Equals(p.Id) && userId.Equals(p.OwnerId));
        if (product is null)
            return response.Failed(false, "Product could not be found");

        if (!string.IsNullOrWhiteSpace(product.Content))
            await DeleteProductContent(product.Content);

        _dbContext.Products.Remove(product);

        await _dbContext.SaveChangesAsync();

        return response.Succeed(true);
    }

    public async Task<ServiceResponse<GetProductFullDto>> GetProductById(Guid productId)
    {
        var serviceResponse = new ServiceResponse<GetProductFullDto>();

        var product = await _dbContext.Products.Include(p => p.Currency)
                                               .Include(p => p.Owner)
                                               .Include(p => p.Category)
                                               .Include(p => p.Tags)
                                               .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return serviceResponse.Failed(null, "Product could not be found");

        product.Owner.Products = [];
        product.Category.Products = [];
        product.Category.Users = [];

        var productDto = new GetProductFullDto(Id: product.Id, product.Title, product.Description, product.Content, product.Price, product.Currency, product.Category, product.Tags.Select(t => t.Name).ToArray(), product.AdStatus);

        return serviceResponse.Succeed(productDto);
    }

    public async Task<ServiceResponse<IEnumerable<GetProductDto>>> GetProducts()
    {
        var serviceResponse = new ServiceResponse<IEnumerable<GetProductDto>>();

        var products = (await _dbContext.Products.Where(p => p.AdStatus != AdStatus.Hidden).Take(50).ToArrayAsync())
            .Select(p => new GetProductDto(
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

    public async Task<ServiceResponse<IEnumerable<GetProductDto>>> GetUsersProducts(Guid userId)
    {
        var serviceResponse = new ServiceResponse<IEnumerable<GetProductDto>>();

        var products = (await _dbContext.Products.Where(p => p.OwnerId == userId).Take(50).ToArrayAsync())
                .Select(p => new GetProductDto(
                Id: p.Id,
                Title: p.Title,
                Currency: p.Currency,
                Price: p.Price,
                Category: p.Category,
                AdStatus: p.AdStatus));

        return serviceResponse.Succeed(products);
    }

    public Task<ServiceResponse<Guid?>> UpdateProduct(Guid productId, UpdateProductDto updateProduct)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<string?>> UploadProductContent(Stream fileStream, string fileName)
    {
        var response = new ServiceResponse<string?>();

        var fileExtension = Path.GetExtension(fileName);

        fileName = Path.GetFileNameWithoutExtension(fileName);
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '-');
        }

        fileName = $"{fileName}-{Guid.NewGuid()}";
        // combine new name and extension
        var newFileName = fileName + fileExtension;

        await _storage.UploadAsync(fileStream, $"/main-storage-45/user-content/{newFileName}");

        return response.Succeed(newFileName);
    }

    public async Task DeleteProductContent(string fileName)
    {
        await _storage.DeleteObjectAsync($"/main-storage-45/user-content/{fileName}");
    }

    public async Task<ServiceResponse<Stream>> DownloadProductContent(string fileName)
    {
        var response = new ServiceResponse<Stream>();

        var stream = await _storage.DownloadObjectAsStreamAsync($"/main-storage-45/user-content/{fileName}");

        if (stream == null)
        {
            return response.Failed(null, "Content could not be found");
        }

        return response.Succeed(stream);
    }

    public async Task<ServiceResponse<Guid?>> UpdateProductContent(Guid userId, Guid productId, Stream content)
    {
        var response = new ServiceResponse<Guid?>();

        var product = await _dbContext.Products.FirstOrDefaultAsync(p => productId.Equals(p.Id) && userId.Equals(p.OwnerId));
        if (product is null)
            return response.Failed(null, "");

        var contentUpload = await UploadProductContent(content, product.Title);
        if (!contentUpload.Success || string.IsNullOrWhiteSpace(contentUpload.Data))
            return response.Failed(null, contentUpload.Error!);
        

        if (!string.IsNullOrWhiteSpace(product.Content))
            await DeleteProductContent(product.Content);

        product.Content = contentUpload.Data;

        await _dbContext.SaveChangesAsync();

        return response.Succeed(productId);
    }

    public async Task<ServiceResponse<Stream>> DownloadProductContent(Guid productId)
    {
        var response = new ServiceResponse<Stream>();

        var product = await _dbContext.Products.FindAsync(productId);
        if (product is null)
            return response.Failed(null, "Product could not be found");

        if (string.IsNullOrWhiteSpace(product.Content))
            return response.Failed(null, "Product does not have uploaded content yet");

        var contentResponse = await DownloadProductContent(product.Content);

        return contentResponse;
    }
}
