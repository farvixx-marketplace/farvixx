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
}
