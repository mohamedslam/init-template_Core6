namespace Fab.Infrastructure.Interfaces.Authentication;

public interface IEncryptionAndDecryptionService
{
    public string Encrypt(string NormalText, string passPhrase);
    public string Decrypt(string EncryptionText, string passPhrase); 
}