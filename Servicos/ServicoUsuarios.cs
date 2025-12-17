using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace SistemaWorkspace.Servicos;

public class ServicoUsuarios
{
    private readonly string _arquivoUsuarios;

    public ServicoUsuarios(IWebHostEnvironment env)
    {
        _arquivoUsuarios = Path.Combine(
            AppContext.BaseDirectory,
            "Dados",
            "usuarios.json"
        );
    }
    private List<UsuarioSistema> LerUsuarios()
    {
        if (!File.Exists(_arquivoUsuarios))
            return new List<UsuarioSistema>();

        var json = File.ReadAllText(_arquivoUsuarios);

        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind == JsonValueKind.Object &&
            doc.RootElement.TryGetProperty("emails", out var emails))
        {
            return JsonSerializer.Deserialize<List<UsuarioSistema>>(
                emails.GetRawText()
            ) ?? new List<UsuarioSistema>();
        }

        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<UsuarioSistema>>(json)
                   ?? new List<UsuarioSistema>();
        }

        return new List<UsuarioSistema>();
    }

    private void SalvarUsuarios(List<UsuarioSistema> usuarios)
    {
        var estrutura = new
        {
            emails = usuarios
        };

        var json = JsonSerializer.Serialize(estrutura, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_arquivoUsuarios, json);
    }
    public UsuarioSistema? BuscarPorEmail(string email)
    {
        email = email.Trim().ToLower();

        return LerUsuarios()
            .FirstOrDefault(u =>
                u.Email.Trim().ToLower() == email
            );
    }

    public bool ValidarSenha(string email, string senha)
    {
        var usuario = BuscarPorEmail(email);
        if (usuario == null || !usuario.SenhaDefinida)
            return false;

        return usuario.HashSenha == GerarHash(senha);
    }

    public void DefinirSenha(string email, string novaSenha)
    {
        var usuarios = LerUsuarios();
        email = email.Trim().ToLower();

        var usuario = usuarios
            .FirstOrDefault(u => u.Email.ToLower() == email);

        if (usuario == null) return;

        usuario.HashSenha = GerarHash(novaSenha);
        usuario.SenhaDefinida = true;

        SalvarUsuarios(usuarios);
    }

    private string GerarHash(string senha)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
public class UsuarioSistema
{
    public string Email { get; set; } = "";
    public bool SenhaDefinida { get; set; }
    public string HashSenha { get; set; } = "";
}
