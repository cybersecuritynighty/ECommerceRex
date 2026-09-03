using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models.ViewModels;

public class UserCreateViewModel
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? Role { get; set; } = "User";
}
