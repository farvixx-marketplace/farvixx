using DigitalMarketplace.Core.DTOs;
using DigitalMarketplace.Core.DTOs.Auth;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Core.Services;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace DigitalMarketplace.Infrastructure.Services;
public class AuthService(
    UserManager<User> userManager,
    ITokenService tokenService,
    IConfiguration configuration,
    ApplicationDbContext dbContext) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<ServiceResponse<Tokens>> Login(LoginUserDto loginUserDto)
    {
        var serviceResponse = new ServiceResponse<Tokens>();

        var user = await _userManager.FindByNameAsync(loginUserDto.Username)
            ?? await _userManager.FindByEmailAsync(loginUserDto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginUserDto.Password))
        {
            return serviceResponse.Failed(null, "Invalid username/email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var tokens = _tokenService.GenerateTokens(User.GetUserDto(user), roles);

        return serviceResponse.Succeed(tokens);
    }

    public async Task<ServiceResponse<Tokens>> Register(RegisterUserDto registerUserDto)
    {
        var serviceResponse = new ServiceResponse<Tokens>();

        if ((await _userManager.FindByNameAsync(registerUserDto.Username)) is not null)
            return serviceResponse.Failed(null,"Username is already taken");

        if ((await _userManager.FindByEmailAsync(registerUserDto.Email)) is not null)
            return serviceResponse.Failed(null, "Email is already taken");

        User? user = new User
        {
            FirstName = registerUserDto.FirstName ?? "Unnamed",
            LastName = registerUserDto.LastName ?? "Unnamed",
            UserName = registerUserDto.Username,
            Email = registerUserDto.Email
    };

        var creationResult = await _userManager.CreateAsync(user, registerUserDto.Password);
        user.Currency = await _dbContext.Currency.FindAsync("USD");

        user = await _userManager.FindByNameAsync(registerUserDto.Username);
        if (!creationResult.Succeeded || user is null)
            return serviceResponse.Failed(
                        null,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed: {0}",
                            string.Join(", ",
                                        creationResult.Errors.Select(e => $"Code: {e.Code} Description: {e.Description}"))));

        var roles = await _userManager.GetRolesAsync(user);
        var tokens = _tokenService.GenerateTokens(User.GetUserDto(user), roles);

        await _dbContext.SaveChangesAsync();

        return serviceResponse.Succeed(tokens);
    }

    public async Task<ServiceResponse<Tokens>> Refresh(string refreshToken)
    {
        var serviceResponse = new ServiceResponse<Tokens>();

        var validationResult = await _tokenService.ValidateToken(refreshToken);
        if (!validationResult.IsValid)
            return serviceResponse.Failed(null, "Refresh token is not valid");
        

        var claims = _tokenService.GeTokenClaims(refreshToken);

        var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.Sub)?.Value;
        if (userId == null)
            return serviceResponse.Failed(null, "Refresh token is not valid");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return serviceResponse.Failed(null, "User with given id could not be found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return serviceResponse.Succeed(_tokenService.GenerateTokens(User.GetUserDto(user), roles));
    }

    public Task<ServiceResponse<bool>> InvalidateTokens(string? accessToken = null, string? refreshToken = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<Tokens>> ExternalLogin(string loginProvider, string credentialsResponseToken)
    {
        var serviceResponse = new ServiceResponse<Tokens>();

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateLifetime = true
        };

        // Validate token lifetime
        var validationResult = await tokenHandler.ValidateTokenAsync(credentialsResponseToken, validationParams);
        if (!validationResult.IsValid)
            return serviceResponse.Failed(null, "Token is expired");


        var claims = tokenHandler.ReadJwtToken(credentialsResponseToken).Claims;
        // Find unique user id from external provider
        var externalUserId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.Sub)?.Value ?? null;
        if (externalUserId == null)
            return serviceResponse.Failed(null, "Invalid token");

        // Find user that connected to this external user
        var user = await _userManager.FindByLoginAsync(loginProvider, externalUserId);

        if (user is null)
        {
            // If null look if user with other provided info (email) exists
            // So we can link this external login to an existing account
            var userEmail = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.Email)?.Value;
            if (userEmail != null && user == null)
                user = await _userManager.FindByEmailAsync(userEmail);

            if (user is null)
            {
                // If user with given email does not exist we create new user
                var firstName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.GivenName)?.Value;
                var lastName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.FamilyName)?.Value;
                
                var fullNameClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaims.Name);
                if (firstName == null && fullNameClaim != null)
                    firstName = fullNameClaim.Value.Split(separator: ' ', count: 1)[0];

                if (lastName == null && fullNameClaim != null)
                    lastName = fullNameClaim.Value.Split(separator: ' ', count: 1)[^1];


                user = new User
                {
                    Email = userEmail,
                    FirstName = firstName ?? "Unnamed",
                    LastName = lastName ?? "Unnamed",
                    ImageUri = claims.FirstOrDefault(c => c.Type == "picture")?.Value
                };

                if (firstName != null && lastName != null)
                {
                    user.UserName = $"{firstName}-{lastName}-{Guid.NewGuid().ToString()[..6]}";
                }

                var creationResult = await _userManager.CreateAsync(user!);
                if (!creationResult.Succeeded)
                {
                    return serviceResponse.Failed(
                        null,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Failed: {0}",
                            string.Join(", ",
                                        creationResult.Errors.Select(e => $"Code: {e.Code} Description: {e.Description}"))));
                }
            }

            await _userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, loginProvider, externalUserId));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tokens = _tokenService.GenerateTokens(User.GetUserDto(user), roles);

        return serviceResponse.Succeed(tokens);
    }
}
