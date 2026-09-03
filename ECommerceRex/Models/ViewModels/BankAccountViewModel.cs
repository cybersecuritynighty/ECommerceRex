using System.ComponentModel.DataAnnotations;

namespace ECommerceRex.Models.ViewModels;

public class BankAccountViewModel
{
    // User details
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    // Bank account info
    public decimal Balance { get; set; }
    public string? Currency { get; set; }

    // Credit card (we don't store CVV, only last 4 digits for display)
    [CreditCard]
    public string? CardNumber { get; set; } // we'll store encrypted or tokenized in real app
    public string? CardExpiry { get; set; } // MM/YY
    public string? CardHolderName { get; set; }
}
