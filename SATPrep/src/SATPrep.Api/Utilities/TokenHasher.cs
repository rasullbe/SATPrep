using System.Security.Cryptography;
using System.Text;

namespace SATPrep.Api.Utilities;

public static class TokenHasher
{
    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public static string GenerateCsrfToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}