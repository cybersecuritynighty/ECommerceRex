using System.Security.Cryptography;
using System.Text;

namespace ECommerceRex.Services;

public class HmacService : IHmacService
{
    private readonly byte[] _key;

    public HmacService(IConfiguration configuration)
    {
        var secret = configuration["Hmac:SecretKey"] ?? throw new InvalidOperationException("HMAC secret key missing.");
        _key = Encoding.UTF8.GetBytes(secret);
    }

    public string ComputeHash(string data)
    {
        using var hmac = new HMACSHA256(_key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    public bool VerifyHash(string data, string hash)
    {
        var computed = ComputeHash(data);
        return computed == hash;
    }
}
