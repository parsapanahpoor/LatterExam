using System.Security.Cryptography;
using System.Text;

namespace TransactionPrivacyApp;

public sealed class AesGcmEncryptionService : IDisposable
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public AesGcmEncryptionService(byte[] key)
    {
        if (key.Length is not (16 or 24 or 32))
            throw new ArgumentException("AES key must be 128, 192, or 256 bits.", nameof(key));

        _key = key;
    }

    public static AesGcmEncryptionService FromEnvironment()
    {
        var keyBase64 = Environment.GetEnvironmentVariable("TRANSACTION_ENCRYPTION_KEY")
            ?? throw new InvalidOperationException(
                "Environment variable TRANSACTION_ENCRYPTION_KEY is not set.");

        var key = Convert.FromBase64String(keyBase64);
        return new AesGcmEncryptionService(key);
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

    public string Decrypt(ReadOnlySpan<byte> payload)
    {
        if (payload.Length < NonceSize + TagSize)
            throw new CryptographicException("Invalid ciphertext payload.");

        var nonce = payload[..NonceSize];
        var tag = payload.Slice(NonceSize, TagSize);
        var cipher = payload[(NonceSize + TagSize)..];

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

    public void Dispose()
    {
        CryptographicOperations.ZeroMemory(_key);
    }
}
