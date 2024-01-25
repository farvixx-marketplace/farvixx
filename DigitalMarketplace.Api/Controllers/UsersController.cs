using DigitalMarketplace.Core.DTOs;
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
    public async Task<IEnumerable<MinimalUser>> GetUsers(CancellationToken ct) => await _userService.GetUsers(cancellationToken: ct);

    [HttpPost]
    public async Task<int> AddUser(MinimalUser user, CancellationToken ct) => await _userService.AddUser(user, cancellationToken: ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User?>> GetUserById(int id, CancellationToken ct)
    {
        var user = await _userService.GetUser(id, cancellationToken: ct);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<int> DeleteUser(int id, CancellationToken ct)
    {
        return await _userService.DeleteUser(id, cancellationToken: ct);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<int>> UpdateUser(int id, UpdateUserDto updateUserDto, CancellationToken ct)
    {
        updateUserDto = updateUserDto with { Id = id };
        var updatedUserId = await _userService.UpdateUser(updateUserDto, ct);
        if (updatedUserId < 0)
        {
            return NotFound();
        }

        return updatedUserId;
    }
}
