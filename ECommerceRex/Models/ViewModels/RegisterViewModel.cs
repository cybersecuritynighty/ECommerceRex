using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models.ViewModels;

public class RegisterViewModel
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
