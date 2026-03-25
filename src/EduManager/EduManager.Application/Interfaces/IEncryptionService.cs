namespace EduManager.Application.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, string slug, string salt);
    string Decrypt(string cipherText, string slug, string salt);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string HashRefreshToken(string token);
}
