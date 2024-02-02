using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DigitalMarketplace.Infrastructure.Services;
public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public Tokens GenerateTokens(GetUserDto user, string[] roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "Not set"),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var expiresInMinutes = Convert.ToDouble(_configuration["JwtConfig:DurationMinutes"]);

        var accessToken = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:IssuerHttps"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        var refreshToken = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:IssuerHttps"],
            audience: _configuration["JwtConfig:Audience"],
            claims: [new(JwtRegisteredClaimNames.Sub, user.Id.ToString())],
            expires: DateTime.UtcNow.AddMonths(1),
            signingCredentials: credentials
        );

        var tokens = new Tokens
        (
            AccessToken: new JwtSecurityTokenHandler().WriteToken(accessToken),
            ExpiresInMinutes: expiresInMinutes,
            RefreshToken: new JwtSecurityTokenHandler().WriteToken(refreshToken)
        );

        return tokens;
    }
}
