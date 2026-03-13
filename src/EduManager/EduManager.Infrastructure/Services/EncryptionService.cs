using Azure;
using EduManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EduManager.Infrastructure.Services;

public class EncryptionService(IConfiguration configuration) : IEncryptionService
{
    public string Decrypt(string cipherText, string slug, string salt)
    {
        byte[] key = DriveKey(slug, salt);
        byte[] combined = Convert.FromBase64String(cipherText);

        int ivLength = 12;
        int tagLength = 16;
        int cipherLength = combined.Length - (ivLength + tagLength);
        
        byte[] iv = new byte[ivLength];
        byte[] cipherBytes = new byte[cipherLength];
        byte[] tag = new byte[tagLength];

        Buffer.BlockCopy(combined, 0, iv, 0, ivLength);
        Buffer.BlockCopy(combined, ivLength, cipherBytes, 0, cipherLength);
        Buffer.BlockCopy(combined, ivLength+cipherLength, tag, 0, tagLength);

        byte[] plainBytes = new byte[cipherLength];

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);

        aesGcm.Decrypt(iv, cipherBytes, tag, plainBytes);

        var ddd= Encoding.UTF8.GetString(plainBytes);
        return ddd;
    }

    public string Encrypt(string plainText, string slug, string salt)
    {
        byte[] key = DriveKey(slug, salt);
        byte[] iv = new byte[12];
        RandomNumberGenerator.Fill(iv);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tags = new byte[16];

        using var aesGM = new AesGcm(key, tagSizeInBytes: 16) ;
        aesGM.Encrypt(iv, plainBytes, cipherBytes, tags);

        byte[] result = new byte[iv.Length + cipherBytes.Length + tags.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);
        Buffer.BlockCopy(tags, 0, result, iv.Length + cipherBytes.Length, tags.Length);

        return Convert.ToBase64String(result);
    }

    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    private byte[] DriveKey(string slug, string salt)
    {
        var input = Encoding.UTF8.GetBytes($"{slug}-{salt}");
        var masterSecret = Encoding
            .UTF8.GetBytes(configuration["Encryption:MasterSecret"]!);

        return Rfc2898DeriveBytes.Pbkdf2(
            input,
            masterSecret,
            iterations:10000,
            HashAlgorithmName.SHA256,
            outputLength:32);
    }
}
