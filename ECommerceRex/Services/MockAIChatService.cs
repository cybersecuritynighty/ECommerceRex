namespace ECommerceRex.Services;

public class MockAIChatService : IAIChatService
{
    private readonly List<string> _responses = new()
    {
        "I can help you with our products. We have a wide range of electronics and accessories.",
        "Our banking system allows you to manage your wallet, view transactions, and transfer funds.",
        "Attendance tracking uses QR codes for check-in and check-out. You can also view your history.",
        "We offer a secure platform with JWT authentication and HMAC-protected records.",
        "You can contact support via the CRM module or email us at support@ecomrex.com."
    };
    private readonly Random _random = new();

    public Task<string> GetResponseAsync(string userMessage)
    {
        var reply = _responses[_random.Next(_responses.Count)];
        return Task.FromResult(reply);
    }
}
