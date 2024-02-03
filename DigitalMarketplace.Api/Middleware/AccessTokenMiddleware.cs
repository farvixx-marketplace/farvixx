using DigitalMarketplace.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DigitalMarketplace.Api.Middleware;

public class AccessTokenMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;

    public async Task Invoke(HttpContext httpContext)
    {
        var accessToken = (httpContext.Request.Headers.FirstOrDefault(h => h.Key == "Authorization")).Value.ToString().Split(' ')[^1];
        if (string.IsNullOrWhiteSpace(accessToken))
        { 
            await _next(httpContext);
            return;
        }

        var handler = new JwtSecurityTokenHandler();

        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtConfig:Key")!)),
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidIssuers = _configuration.GetSection("JwtConfig:Issuer").Value?.Split(';'),
            ValidAudiences = _configuration.GetSection("JwtConfig:Audience").Value?.Split(';')
        };

        var validationResult = await handler.ValidateTokenAsync(accessToken, validationParameters);
        if (!validationResult.IsValid)
        {
            await _next(httpContext);
            return;
        }

        var claims = handler.ReadJwtToken(accessToken).Claims;

        var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (userId == null)
        {
            await _next(httpContext);
            return;
        }

        httpContext.Items["UserId"] = userId;
        await _next(httpContext);
    }
}
