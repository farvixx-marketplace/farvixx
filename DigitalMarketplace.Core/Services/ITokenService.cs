using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.DTOs.Users;

namespace DigitalMarketplace.Core.Services;
public interface ITokenService
{
    Tokens GenerateTokens(GetUserDto user, string[] roles);
}
