using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.Services;

namespace DigitalMarketplace.Infrastructure.Services;
public class AuthService : IAuthService
{
    public Task<ServiceResponse<Tokens>> Login(LoginUserDto loginUserDto)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<Tokens>> Register(RegisterUserDto registerUserDto)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<Tokens>> Refresh(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InvalidateTokens(string? accessToken = null, string? refreshToken = null)
    {
        throw new NotImplementedException();
    }
}
