using System.Security.Cryptography;
using System.Text;

namespace PasswordSecurityApp;

public static class CredentialStore
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private const char Separator = ':';

    public static void SaveCredentials(string username, string password, string path = "credentials.txt")
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashPassword(password, salt);

        var line = string.Join(Separator,
            username,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));

        File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
    }

    public static bool VerifyCredentials(string username, string password, string path = "credentials.txt")
    {
        if (!File.Exists(path))
            return false;

        foreach (var line in File.ReadLines(path, Encoding.UTF8))
        {
            var parts = line.Split(Separator);
            if (parts.Length != 3 || !parts[0].Equals(username, StringComparison.Ordinal))
                continue;

            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);
            var computedHash = HashPassword(password, salt);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }

        return false;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
    }
}
