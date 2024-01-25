using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.Models;

namespace DigitalMarketplace.Core.Services;
public interface IUserService
{
    Task<IEnumerable<MinimalUser>> GetUsers(CancellationToken cancellationToken = default);
    Task<User?> GetUser(int id = -1, string username = "", string email = "", CancellationToken cancellationToken = default);
    Task<int> AddUser(MinimalUser user, CancellationToken cancellationToken = default);
    Task<int> DeleteUser(int id, CancellationToken cancellationToken = default);
    Task<int> UpdateUser(UpdateUserDto updateUser, CancellationToken cancellationToken = default);
}
