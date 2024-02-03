using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.DTOs.Users;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DigitalMarketplace.Core.Services;
public interface ITokenService
{
    Tokens GenerateTokens(GetUserDto user, IEnumerable<string> roles);
    Task<TokenValidationResult> ValidateToken(string token);
    IEnumerable<Claim> GeTokenClaims(string  token);
}
