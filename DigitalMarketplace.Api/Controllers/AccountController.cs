using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<GetUserFullDto>>> GetUserProfile()
    {
        ServiceResponse<GetUserFullDto> response = new();

        if (HttpContext.Items["UserId"] == null)
        {
            return Unauthorized();
        }
        var userId = (Guid)HttpContext.Items["UserId"]!;

        User? user;
        try
        {
            user = (await _userService.GetUser(id: userId))?.Data;
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }
        if (user == null)
        {
            return NotFound();
        }

        return response.Succeed(new GetUserFullDto(user.Id, 
            user.ImageUri,
            user.FirstName,
            user.LastName,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.Location,
            user.Balance,
            user.Currency,
            user.Gender,
            user.BirthDate,
            user.Languages,
            user.Tags,
            user.Categories,
            user.ExternalResources,
            user.Bio
            ));
    }
}
