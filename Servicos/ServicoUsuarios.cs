using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using SistemaWorkspace.Modelos;

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

        var hash = GerarHashComSalt(senha, usuario.Salt);
        return usuario.HashSenha == hash;
    }

    public void DefinirSenha(string email, string novaSenha)
    {
        var usuarios = LerUsuarios();
        email = email.Trim().ToLower();

        var usuario = usuarios
            .FirstOrDefault(u => u.Email.ToLower() == email);

        if (usuario == null) return;

        var salt = GerarSalt();
        usuario.Salt = salt;
        usuario.HashSenha = GerarHashComSalt(novaSenha, salt);
        usuario.SenhaDefinida = true;

        SalvarUsuarios(usuarios);
    }

    public void AtualizarUsuario(UsuarioSistema usuario)
    {
        var usuarios = LerUsuarios();
        var idx = usuarios.FindIndex(u => u.Email.Trim().ToLower() == usuario.Email.Trim().ToLower());
        if (idx >= 0)
        {
            usuarios[idx] = usuario;
            SalvarUsuarios(usuarios);
        }
    }

    private string GerarSalt()
    {
        var bytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string GerarHashComSalt(string senha, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        // PBKDF2 com HMACSHA256
        var derived = Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
            password: senha,
            salt: saltBytes,
            prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        return Convert.ToBase64String(derived);
    }
}
