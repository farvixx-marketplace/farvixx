using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMarketplace.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet]
    public async Task<ServiceResponse<IEnumerable<GetUserDto>>> GetUsers(CancellationToken ct) => await _userService.GetUsers(cancellationToken: ct);

    [HttpPost]
    public async Task<ServiceResponse<Guid>> AddUser(AddUserDto user, CancellationToken ct)
    {
        // TODO: Add Checks About ServiceResponse

        return await _userService.AddUser(user, cancellationToken: ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User?>> GetUserById(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetUser(id, cancellationToken: ct);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<Guid> DeleteUser(Guid id, CancellationToken ct)
    {
        // TODO: Add ServiceResponse Checks

        return (await _userService.DeleteUser(id, cancellationToken: ct)).Data;
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateUser(Guid id, UpdateUserDto updateUserDto, CancellationToken ct)
    {
        var updatedUserId = await _userService.UpdateUser(id, updateUserDto, ct);
        if (!updatedUserId.Success || updatedUserId.Data == Guid.Empty)
        {
            return NotFound(updatedUserId.Error);
        }

        return updatedUserId.Data;
    }
}
