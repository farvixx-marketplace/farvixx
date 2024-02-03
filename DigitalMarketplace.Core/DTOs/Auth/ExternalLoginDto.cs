namespace DigitalMarketplace.Core.DTOs.Auth;
public record ExternalLoginDto(
    string LoginProvider,
    string CredentialResponseToken);
