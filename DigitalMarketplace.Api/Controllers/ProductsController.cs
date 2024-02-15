using BunnyCDN.Net.Storage;
using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Products;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductService productService,
    BunnyCDNStorage bunnyCDNStorage) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly BunnyCDNStorage _storage = bunnyCDNStorage;

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

    [HttpPost("upload-content")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadContent(IFormFile formFile)
    {
        var stream = formFile.OpenReadStream();

        var response = await _productService.UploadProductContent(stream, formFile.FileName);

        return Ok(response);
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
