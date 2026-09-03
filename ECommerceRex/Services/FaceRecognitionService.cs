using System.Text;
using System.Text.Json;

namespace ECommerceRex.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FaceRecognitionService> _logger;

    public FaceRecognitionService(HttpClient httpClient, ILogger<FaceRecognitionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<float[]> RegisterFaceAsync(int userId, string imageBase64)
    {
        var payload = new { image = imageBase64, user_id = userId };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/register", content);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(resultJson);
        var encoding = doc.RootElement.GetProperty("encoding").EnumerateArray().Select(x => (float)x.GetDouble()).ToArray();
        return encoding;
    }

    public async Task<int?> RecognizeFaceAsync(string imageBase64, List<UserFaceEncoding> knownEncodings)
    {
        var payload = new
        {
            image = imageBase64,
            known_encodings = knownEncodings.Select(e => new { user_id = e.UserId, encoding = e.Encoding })
        };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/recognize", content);
        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resultJson);
            var userId = doc.RootElement.GetProperty("user_id").GetInt32();
            return userId;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        else
        {
            response.EnsureSuccessStatusCode();
            return null;
        }
    }
}
