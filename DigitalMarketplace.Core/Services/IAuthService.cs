using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;

namespace DigitalMarketplace.Core.Services;
public interface IAuthService
{
    Task<ServiceResponse<Tokens>> Login(LoginUserDto loginUserDto);
    Task<ServiceResponse<Tokens>> Refresh(string refreshToken);
    Task<ServiceResponse<Tokens>> Register(RegisterUserDto registerUserDto);
    Task<bool> InvalidateTokens(string? accessToken = null, string? refreshToken = null);
}
