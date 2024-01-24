using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
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
    public async Task<User?> GetUserById(int id, CancellationToken ct)
    {
        return await _userService.GetUser(id, cancellationToken: ct);
    }

    [HttpDelete("{id:int}")]
    public async Task<int> DeleteUserById(int id, CancellationToken ct)
    {
        return await _userService.DeleteUser(id, cancellationToken: ct);
    }
}
