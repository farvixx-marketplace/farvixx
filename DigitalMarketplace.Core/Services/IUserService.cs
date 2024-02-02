using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Models;

namespace DigitalMarketplace.Core.Services;
public interface IUserService
{
    Task<ServiceResponse<IEnumerable<GetUserDto>>> GetUsers(CancellationToken cancellationToken = default);
    Task<ServiceResponse<User>> GetUser(Guid? id = null, string? username = null, string? email = null, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> AddUser(AddUserDto user, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResponse<Guid>> UpdateUser(Guid id, UpdateUserDto updateUser, CancellationToken cancellationToken = default);
}
