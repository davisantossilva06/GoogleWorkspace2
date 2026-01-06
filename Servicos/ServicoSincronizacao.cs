using System.Text.Json;

namespace SistemaWorkspace.Servicos;

public class ServicoSincronizacao
{
    private readonly ServicoGoogleWorkspace _workspace;
    private readonly string _arquivoCache;

    public ServicoSincronizacao(ServicoGoogleWorkspace workspace)
    {
        _workspace = workspace;
        _arquivoCache = Path.Combine(AppContext.BaseDirectory, "Dados", "usuarios_workspace.json");
    }

    public void SincronizarAgora()
    {
        var usuarios = _workspace.ListarUsuarios();
        var json = JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_arquivoCache, json);
    }

    public List<object> LerCache()
    {
        if (!File.Exists(_arquivoCache)) return new List<object>();
        var json = File.ReadAllText(_arquivoCache);
        return JsonSerializer.Deserialize<List<object>>(json) ?? new List<object>();
    }
}
