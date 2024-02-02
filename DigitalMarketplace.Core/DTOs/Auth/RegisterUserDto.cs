using System.ComponentModel.DataAnnotations;

namespace DigitalMarketplace.Core.DTOs.Auth;
public record RegisterUserDto(
    string? FirstName,
    string? LastName,
    [Required(ErrorMessage ="Username is required")]
    string Username,
    [Required(ErrorMessage ="Email is required")]
    string Email,
    [Required(ErrorMessage ="Password is required")]
    string Password);
