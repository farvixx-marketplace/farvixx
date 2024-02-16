using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<GetProductDto>>>> GetProductsAsync() => await _productService.GetProducts();

    [HttpGet("id/{productId:guid}")]
    public async Task<ActionResult<ServiceResponse<GetProductFullDto>>> GetProductById(Guid productId)
    {
        ServiceResponse<GetProductFullDto> response;
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromForm]AddProductDto addProduct, IFormFile content)
    {
        var userId = HttpContext.Items["UserId"];
        if (userId == null)
        {
            return Unauthorized();
        }
        try
        {
            var productCreated = await _productService.AddProduct((Guid)userId, addProduct, content.OpenReadStream());
            if (productCreated == null || !productCreated.Success)
            {
                return BadRequest(productCreated);
            }
            return CreatedAtAction(nameof(CreateProduct), productCreated);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPatch("id/{productId:guid}/content")]
    public async Task<IActionResult> ReplaceProductContent(Guid productId, IFormFile content)
    {
        if (HttpContext.Items["UserId"] is null)
            return Unauthorized();
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var updateRespose = await _productService.UpdateProductContent(userId, productId, content.OpenReadStream());
        if (updateRespose.Success)
            return Ok(updateRespose);

        return BadRequest(updateRespose);
    }

    [HttpGet("download-content")]
    public async Task<IActionResult> DownloadContent(string fileName)
    {
        var response = await _productService.DownloadProductContent(fileName);
        if (response.Success && response.Data is not null)
            return File(response.Data, "application/octet-stream", fileName);

        return NotFound();
    }
}
