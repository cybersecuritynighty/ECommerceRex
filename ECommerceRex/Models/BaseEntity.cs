namespace ECommerceRex.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public string? RowHash { get; set; }          // HMAC-SHA256 of all fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
