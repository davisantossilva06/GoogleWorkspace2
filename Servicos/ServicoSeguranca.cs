using System.Security.Cryptography;
using System.Text;

namespace SistemaWorkspace.Servicos;

public class ServicoSeguranca
{
    public string GerarSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public string GerarHash(string senha, string salt)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha + salt);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
