namespace ECommerceRex.Services;

public interface IHmacService
{
    string ComputeHash(string data);
    bool VerifyHash(string data, string hash);
}
