namespace ECommerceRex.Services;

public interface IAIChatService
{
    Task<string> GetResponseAsync(string userMessage);
}
