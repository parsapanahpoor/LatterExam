using System.Security.Cryptography;
using System.Text;

namespace SensitiveDataEncryptionApp;

public sealed class SecureAesService : IDisposable
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    private SecureAesService(byte[] key) => _key = key;

    public static SecureAesService FromEnvironment()
    {
        var keyBase64 = Environment.GetEnvironmentVariable("SENSITIVE_DATA_KEY");
        if (string.IsNullOrWhiteSpace(keyBase64))
        {
            var generated = RandomNumberGenerator.GetBytes(32);
            keyBase64 = Convert.ToBase64String(generated);
            Console.WriteLine("SENSITIVE_DATA_KEY was not set. Generated a runtime-only key for this session.");
            Console.WriteLine($"Set SENSITIVE_DATA_KEY={keyBase64} for persistent encryption.");
        }

        var key = Convert.FromBase64String(keyBase64);
        if (key.Length is not (16 or 24 or 32))
            throw new InvalidOperationException("Key must be 128, 192, or 256 bits.");

        return new SecureAesService(key);
    }

    public byte[] Encrypt(string plaintext)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        try
        {
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var cipher = new byte[plainBytes.Length];
            var tag = new byte[TagSize];

            using var aes = new AesGcm(_key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipher, tag);

            var payload = new byte[NonceSize + TagSize + cipher.Length];
            nonce.CopyTo(payload.AsSpan(0, NonceSize));
            tag.CopyTo(payload.AsSpan(NonceSize, TagSize));
            cipher.CopyTo(payload.AsSpan(NonceSize + TagSize));
            return payload;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    public string Decrypt(byte[] payload)
    {
        var nonce = payload.AsSpan(0, NonceSize);
        var tag = payload.AsSpan(NonceSize, TagSize);
        var cipher = payload.AsSpan(NonceSize + TagSize);

        var plainBytes = new byte[cipher.Length];
        try
        {
            using var aes = new AesGcm(_key, TagSize);
            aes.Decrypt(nonce, cipher, tag, plainBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    public void Dispose() => CryptographicOperations.ZeroMemory(_key);
}

public static class SensitiveDataStore
{
    private const string StorePath = "sensitive-data.bin";

    public static void Save(byte[] encryptedPayload) =>
        File.WriteAllBytes(StorePath, encryptedPayload);

    public static byte[] Load()
    {
        if (!File.Exists(StorePath))
            throw new FileNotFoundException("No encrypted data file found.", StorePath);

        return File.ReadAllBytes(StorePath);
    }
}
