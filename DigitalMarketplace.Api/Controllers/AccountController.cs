using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserService userService,
    IEmailService emailSender) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IEmailService _emailSender = emailSender;

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

        try
        {
            response = await _userService.GetUser(id: userId);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }
        if (response == null)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("update-password")]
    [Authorize]
    public async Task<ActionResult> UpdatePassword([FromForm] string oldPassword, [FromForm] string newPassword)
    {
        Guid id = (Guid)HttpContext.Items["UserId"]!;
        ServiceResponse<Guid?> response;

        try
        {
            response = await _userService.UpdatePassword(id, oldPassword, newPassword);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("update-email")]
    [Authorize]
    public async Task<ActionResult> UpdateEmail([FromForm] string email)
    {
        Guid id = (Guid)HttpContext.Items["UserId"]!;
        ServiceResponse<Guid?> response;

        try
        {
            response = await _userService.UpdateEmail(id, email);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("update-username")]
    [Authorize]
    public async Task<ActionResult> UpdateUsername([FromForm] string username)
    {
        Guid id = (Guid)HttpContext.Items["UserId"]!;
        ServiceResponse<Guid?> response;

        try
        {
            response = await _userService.UpdateUsername(id, username);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpGet("confirm-email")]
    [Authorize]
    public async Task<IActionResult> ConfirmUserEmail(string token)
    {
        var result = await _userService.ConfirmEmail((Guid)HttpContext.Items["UserId"]!, token);

        return Ok(result);
    }
}
