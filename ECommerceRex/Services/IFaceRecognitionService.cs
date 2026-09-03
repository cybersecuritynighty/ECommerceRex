using System.Text.Json;

namespace ECommerceRex.Services;

public interface IFaceRecognitionService
{
    Task<float[]> RegisterFaceAsync(int userId, string imageBase64);
    Task<int?> RecognizeFaceAsync(string imageBase64, List<UserFaceEncoding> knownEncodings);
}

public class UserFaceEncoding
{
    public int UserId { get; set; }
    public float[] Encoding { get; set; } = Array.Empty<float>();
}
