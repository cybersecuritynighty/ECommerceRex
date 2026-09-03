using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models.ViewModels;

public class UserEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? Password { get; set; }  // optional – leave blank to keep current

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    public string? FullName { get; set; }
    public string? Role { get; set; }
}
