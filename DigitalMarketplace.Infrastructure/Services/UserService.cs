using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketplace.Infrastructure.Services;
public class UserService(ApplicationDbContext dbContext) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<ServiceResponse<Guid>> AddUser(AddUserDto newUser, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<Guid>();
        var user = new User
        {
            ImageUri = newUser.ImageUri ?? "",
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            UserName = $"{newUser.FirstName}-{newUser.LastName}-{Guid.NewGuid().ToString()[..8]}",
            Email = newUser.Email,
        };
        var currencyExists = await _dbContext.Currency.FindAsync([user.Currency.Name], cancellationToken: ct);
        if (currencyExists != null)
        {
            user.Currency = currencyExists;
        }

        await _dbContext.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);
        return serviceResponse.Succeed(user.Id);
    }

    public async Task<ServiceResponse<Guid>> DeleteUser(Guid id, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<Guid>();

        var user = await _dbContext.Users.FindAsync([ id ], cancellationToken: ct);
        if (user is null)
            return serviceResponse.Failed(Guid.Empty, "");

        _dbContext.Remove(user);
        await _dbContext.SaveChangesAsync(ct);
        return serviceResponse.Succeed(user.Id);
    }

    public async Task<ServiceResponse<User?>> GetUser(Guid? id = null, string? username = null, string? email = null, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<User?>();

        var users = _dbContext.Users
                .Include(u => u.Products)
                .Include(u => u.Currency);
        if (id is not null && id != Guid.Empty)
            return serviceResponse.Succeed(await users.FirstOrDefaultAsync(u => u.Id == id, ct));

        if (string.IsNullOrWhiteSpace(username))
            return serviceResponse.Succeed(await users.FirstOrDefaultAsync(u => u.UserName!.Equals(username, StringComparison.CurrentCultureIgnoreCase), ct));

        if (string.IsNullOrWhiteSpace(email))
            return serviceResponse.Succeed(await users.FirstOrDefaultAsync(u => u.Email!.Equals(email, StringComparison.CurrentCultureIgnoreCase), ct));

        return null;
    }

    public async Task<ServiceResponse<IEnumerable<GetUserDto>>> GetUsers(CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<IEnumerable<GetUserDto>>();
        var users = await _dbContext.Users.Take(10).ToListAsync(ct);
        return serviceResponse.Succeed(users.Select(User.GetUserDto));
    }

    public async Task<ServiceResponse<Guid>> UpdateUser(Guid id, UpdateUserDto updateUser, CancellationToken ct = default)
    {
        var serviceResponse = new ServiceResponse<Guid>();

        if (Guid.Empty == id)
            return serviceResponse.Failed(Guid.Empty, "");

        var user = await _dbContext.Users
            .Include(u => u.Categories.Where(t => updateUser.CategoryIds != null))
            .Include(u => u.Tags.Where(t => updateUser.Tags != null))
            .FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
            return serviceResponse.Failed(Guid.Empty, "");

        user.Update(updateUser);
        if (updateUser.Tags != null)
        {
            var userTags = _dbContext.Tags.Where(updateUser.Tags.Contains).ToList();
            if (userTags.Count != updateUser.Tags.Count)
            {
                var newUserTags = updateUser.Tags.Where(t => !userTags.Contains(t)).ToList();
                foreach ( var t in newUserTags )
                {
                    _dbContext.Tags.Add(t);
                }
            }
        }


        await _dbContext.SaveChangesAsync(ct);
        return serviceResponse.Succeed(user.Id);
    }
}
