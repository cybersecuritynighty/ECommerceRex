using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models;

public class User : BaseEntity
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty; // store hashed
    public string? FullName { get; set; }
    public string? Role { get; set; } = "User";
    // Relationships
    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<Attendance>? Attendances { get; set; }
}
