namespace ECommerceRex.Models;

public class Attendance : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? ScanCode { get; set; } // for QR/Barcode scan
}
