using System.Text.Json;
using SistemaWorkspace.Modelos;

namespace SistemaWorkspace.Servicos;

public class ServicoUsuarios
{
    private readonly string caminho = "Dados/usuarios.json";
    private readonly ServicoSeguranca seguranca;

    public ServicoUsuarios(ServicoSeguranca s)
    {
        seguranca = s;
    }

    public UsuarioSistema? ObterPorEmail(string email)
    {
        var lista = LerTodos();
        return lista.FirstOrDefault(u => u.Email == email);
    }

    public void DefinirSenha(UsuarioSistema usuario, string senha)
    {
        usuario.Salt = seguranca.GerarSalt();
        usuario.HashSenha = seguranca.GerarHash(senha, usuario.Salt);
        usuario.SenhaDefinida = true;
        Salvar(usuario);
    }

    public bool ValidarSenha(UsuarioSistema usuario, string senha)
    {
        var hash = seguranca.GerarHash(senha, usuario.Salt);
        return hash == usuario.HashSenha;
    }

    private List<UsuarioSistema> LerTodos()
    {
        return JsonSerializer.Deserialize<List<UsuarioSistema>>(
            File.ReadAllText(caminho)
        ) ?? new();
    }

    private void Salvar(UsuarioSistema usuario)
    {
        var lista = LerTodos();
        var indice = lista.FindIndex(u => u.Email == usuario.Email);
        lista[indice] = usuario;

        File.WriteAllText(
            caminho,
            JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true })
        );
    }
}
