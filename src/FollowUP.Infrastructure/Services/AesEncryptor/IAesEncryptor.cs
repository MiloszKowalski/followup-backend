namespace FollowUP.Infrastructure.Services
{
    public interface IAesEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string encryptedText);
    }
}
