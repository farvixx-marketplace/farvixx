using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.DTOs.Users;
using DigitalMarketplace.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace DigitalMarketplace.Infrastructure.Services;
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    private readonly SecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SigningCredentials _credentials;
    public TokenValidationParameters TokenValidationParameters { get; private set; }
    private readonly JwtSecurityTokenHandler _handler;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration.GetSection("JwtConfig");
        if (_configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration), "JWT configuration section was not found");
        }
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]!));

        _issuer = configuration["Issuer"]?.Split(';')[0]!;
        _audience = configuration["Audience"]?.Split(';')[0]!;

        _credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

        TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidIssuers = configuration["Issuer"]?.Split(';'),
            ValidAudiences = configuration["Audience"]?.Split(';')
        };

        _handler = new();
    }

    public Tokens GenerateTokens(GetUserDto user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaims.UniqueName, user.UserName ?? "Username not set"),
            new(JwtRegisteredClaims.Sub, user.Id.ToString()),
        };
        if (!string.IsNullOrWhiteSpace(user.FirstName))
            claims.Add(new(JwtRegisteredClaims.GivenName, user.FirstName));

        if (!string.IsNullOrWhiteSpace(user.LastName))
            claims.Add(new(JwtRegisteredClaims.FamilyName, user.LastName));

        if (!string.IsNullOrWhiteSpace(user.FirstName) 
            && !string.IsNullOrWhiteSpace(user.LastName))
            claims.Add(new(JwtRegisteredClaims.Name, $"{user.FirstName} {user.LastName}"));

        if (!string.IsNullOrWhiteSpace(user.Email))
            claims.Add(new(JwtRegisteredClaims.Email, user.Email));

        if (!string.IsNullOrWhiteSpace(user.ImageUri))
            claims.Add(new("picture", user.ImageUri));

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var expiresInMinutes = Convert.ToDouble(_configuration["JwtConfig:DurationMinutes"]);

        var accessToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: _credentials
        );

        var refreshToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: [new(JwtRegisteredClaims.Sub, user.Id.ToString())],
            expires: DateTime.UtcNow.AddMonths(1),
            signingCredentials: _credentials
        );

        var tokens = new Tokens
        (
            AccessToken: _handler.WriteToken(accessToken),
            ExpiresInMinutes: expiresInMinutes,
            RefreshToken: _handler.WriteToken(refreshToken)
        );

        return tokens;
    }

    public IEnumerable<Claim> GeTokenClaims(string token)
    {
        return _handler.ReadJwtToken(token).Claims;
    }

    public async Task<TokenValidationResult> ValidateToken(string token)
    {
        return await _handler.ValidateTokenAsync(token, TokenValidationParameters);
    }
}
