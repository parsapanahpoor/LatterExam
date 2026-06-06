using System.Security.Cryptography;
using System.Text;

namespace SecureTransactionApp;

public sealed class SecureKeyProvider : IDisposable
{
    private readonly byte[] _key;

    private SecureKeyProvider(byte[] key) => _key = key;

    public static SecureKeyProvider FromEnvironment()
    {
        var keyBase64 = Environment.GetEnvironmentVariable("SECURE_TRANSACTION_KEY")
            ?? throw new InvalidOperationException(
                "Environment variable SECURE_TRANSACTION_KEY is not set.");

        var key = Convert.FromBase64String(keyBase64);
        if (key.Length is not (16 or 24 or 32))
            throw new InvalidOperationException("Key must be 128, 192, or 256 bits.");

        return new SecureKeyProvider(key);
    }

    public byte[] Key => _key;

    public void Dispose() => CryptographicOperations.ZeroMemory(_key);
}

public sealed class TransactionCryptoService : IDisposable
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly SecureKeyProvider _keyProvider;

    public TransactionCryptoService(SecureKeyProvider keyProvider) => _keyProvider = keyProvider;

    public byte[] EncryptTransaction(string accountNumber, decimal amount, string description)
    {
        var payload = $"{accountNumber}|{amount}|{description}|{DateTime.UtcNow:O}";
        var plainBytes = Encoding.UTF8.GetBytes(payload);

        try
        {
            var nonce = RandomNumberGenerator.GetBytes(NonceSize);
            var cipher = new byte[plainBytes.Length];
            var tag = new byte[TagSize];

            using var aes = new AesGcm(_keyProvider.Key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipher, tag);

            var result = new byte[NonceSize + TagSize + cipher.Length];
            nonce.CopyTo(result.AsSpan(0, NonceSize));
            tag.CopyTo(result.AsSpan(NonceSize, TagSize));
            cipher.CopyTo(result.AsSpan(NonceSize + TagSize));
            return result;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    public string DecryptTransaction(ReadOnlySpan<byte> encryptedPayload)
    {
        var nonce = encryptedPayload[..NonceSize];
        var tag = encryptedPayload.Slice(NonceSize, TagSize);
        var cipher = encryptedPayload[(NonceSize + TagSize)..];

        var plainBytes = new byte[cipher.Length];
        try
        {
            using var aes = new AesGcm(_keyProvider.Key, TagSize);
            aes.Decrypt(nonce, cipher, tag, plainBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    public void Dispose() => _keyProvider.Dispose();
}
