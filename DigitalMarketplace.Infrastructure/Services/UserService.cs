using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMarketplace.Infrastructure.Services;
public class UserService(ApplicationDbContext dbContext) : IUserService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> AddUser(MinimalUser newUser, CancellationToken ct = default)
    {
        var user = new User
        {
            ImageUri = newUser.ImageUri,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Username = newUser.Username,
            Email = newUser.Email,
        };
        var currencyExists = await _dbContext.Currency.FindAsync(user.Currency.Name);
        if (currencyExists != null)
        {
            user.Currency = currencyExists;
        }

        await _dbContext.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<int> DeleteUser(int id, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FindAsync([ id ], cancellationToken: ct);
        if (user is null)
            return -1;

        _dbContext.Remove(user);
        await _dbContext.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<User?> GetUser(int id = -1, string username = "", string email = "", CancellationToken ct = default)
    {
        var users = _dbContext.Users
                .Include(u => u.Products)
                .Include(u => u.Currency);
        if (id > 0)
            return await users.FirstOrDefaultAsync(u => u.Id == id, ct);

        if (username != "")
            return await users.FirstOrDefaultAsync(u => u.Username == username, ct);

        if (email != "")
            return await users.FirstOrDefaultAsync(u => u.Email == email, ct);

        return null;
    }

    public async Task<IEnumerable<MinimalUser>> GetUsers(CancellationToken ct = default)
    {
        var users = await _dbContext.Users.Take(10).ToListAsync(ct);
        return users.Select(User.GetMinimal);
    }

    public async Task<int> UpdateUser(UpdateUserDto updateUser, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Categories.Where(t => updateUser.CategoryIds != null))
            .Include(u => u.Tags.Where(t => updateUser.Tags != null))
            .FirstOrDefaultAsync(u => u.Id == updateUser.Id, ct);
        if (user is null)
            return -1;

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
        return user.Id;
    }
}
