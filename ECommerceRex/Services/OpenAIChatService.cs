using System.Text;
using System.Text.Json;

namespace ECommerceRex.Services;

public class OpenAIChatService : IAIChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _endpoint;

    public OpenAIChatService(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key missing.");
        _model = config["OpenAI:Model"] ?? "gpt-3.5-turbo";
        _endpoint = config["OpenAI:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        var request = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant for an e‑commerce platform called E‑Commerce Rex. Provide concise, informative answers about products, banking, attendance, and general inquiries." },
                new { role = "user", content = userMessage }
            },
            max_tokens = 300,
            temperature = 0.7
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        var reply = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return reply ?? "I'm sorry, I didn't understand that.";
    }
}
