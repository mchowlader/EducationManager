namespace EduManager.Application.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText, string slug, string salt);
    string Decrypt(string cipherText, string slug, string salt);
}
