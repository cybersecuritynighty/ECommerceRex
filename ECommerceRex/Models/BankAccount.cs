using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceRex.Models;

public class BankAccount : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }
    public string? Currency { get; set; } = "USD";
}
