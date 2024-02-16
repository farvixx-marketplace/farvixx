using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<Tokens>>> Login([FromForm] LoginUserDto loginUser)
    {
        ServiceResponse<Tokens> response;
        try
        {
            response = await _authService.Login(loginUser);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<Tokens>>> Register([FromForm] RegisterUserDto registerUser)
    {
        ServiceResponse<Tokens> response;
        try
        {
            response = await _authService.Register(registerUser);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
            return BadRequest(response);

        return StatusCode(201, response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ServiceResponse<Tokens>>> Refresh(string refreshToken)
    {
        ServiceResponse<Tokens> response;
        try
        {
            response = await _authService.Refresh(refreshToken);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("signin-{loginProvider}")]
    public async Task<ActionResult<ServiceResponse<Tokens>>> ExternalLogin(string loginProvider, string credentialResponse)
    {
        ServiceResponse<Tokens> response;
        try
        {
            response = await _authService.ExternalLogin(loginProvider, credentialResponse);
        }
        catch (Exception)
        {
            return StatusCode(500, "Please try again later. We are trying to resolve the issue");
        }

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }
}
