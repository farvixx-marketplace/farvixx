using System.ComponentModel.DataAnnotations;

namespace DigitalMarketplace.Core.DTOs.Auth;
public record LoginUserDto(
    [Required(ErrorMessage ="Username or email is required")]
    string Username,
    [Required(ErrorMessage ="Password is required")]
    string Password);