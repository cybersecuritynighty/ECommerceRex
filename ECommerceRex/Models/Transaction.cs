using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceRex.Models;

public class Transaction : BaseEntity
{
    public int SenderId { get; set; }
    public User? Sender { get; set; }
    public int? ReceiverId { get; set; }
    public User? Receiver { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public string? Type { get; set; } // "Deposit", "Withdrawal", "Transfer", "Purchase"
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Status { get; set; } = "Pending";
}
