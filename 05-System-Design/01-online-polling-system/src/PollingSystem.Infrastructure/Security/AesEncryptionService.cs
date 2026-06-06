using System.Security.Cryptography;
using System.Text;
using PollingSystem.Application.Interfaces;

namespace PollingSystem.Infrastructure.Security;

/// <summary>
/// مفهوم ذخیره‌سازی رمزنگاری‌شده — در production از Azure Key Vault یا HSM استفاده شود.
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService()
    {
        // کلید ثابت فقط برای demo — در production از configuration امن بخوانید
        _key = SHA256.HashData(Encoding.UTF8.GetBytes("PollingSystem-Demo-Key-2026"));
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
    }

    public string Decrypt(string cipherText)
    {
        var fullBytes = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = fullBytes.Take(16).ToArray();
        var cipherBytes = fullBytes.Skip(16).ToArray();
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
