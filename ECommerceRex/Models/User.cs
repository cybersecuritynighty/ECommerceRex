using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models;

public class User : BaseEntity
{

    public int UserId { get; set; }
    public string? TelegramId { get; set; }
    public string? FaceEmbeddings { get; set; }
    
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; }
    public string? Address { get; set; }
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = "User";
    
    // Relationships
    public ICollection<Transaction>? Transactions { get; set; }
    public ICollection<Attendance>? Attendances { get; set; }
}
