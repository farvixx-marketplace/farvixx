using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetProductDto>>>> GetProductsAsync() => await _productService.GetProducts();

    [HttpGet("id/{productId:guid}")]
    public async Task<ActionResult<ServiceResponse<GetProductFullDto>>> GetProductById(Guid productId)
    {
        var response = new ServiceResponse<GetProductFullDto>();
        try
        {
            response = await _productService.GetProductById(productId);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (response.Success && response.Data is null)
            return NotFound(response);

        if (!response.Success)
        {
            return StatusCode(500, response.Error);
        }

        return response;
    }
}
