namespace DigitalMarketplace.Core.DTOs.Auth;
public record Tokens(
    string AccessToken,
    double ExpiresInMinutes,
    string RefreshToken);