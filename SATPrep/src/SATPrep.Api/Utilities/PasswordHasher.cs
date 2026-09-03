namespace SATPrep.Api.Utilities;

public static class PasswordHasher
{
    private const int SaltSize = 12;
    private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("dummy", SaltSize);
    public static string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, SaltSize);

    public static bool Verify(string hash, string password)
        => BCrypt.Net.BCrypt.Verify(password, hash);

    public static void VerifyDummy(string password)
        => BCrypt.Net.BCrypt.Verify(password, DummyHash);
}