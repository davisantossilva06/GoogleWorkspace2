using System.Text.Json;
using SistemaWorkspace.Modelos;

namespace SistemaWorkspace.Servicos;

public class ServicoMaquinas
{
    public bool EstaAutorizada(string ip)
    {
        var lista = JsonSerializer.Deserialize<List<MaquinaAutorizada>>(
            File.ReadAllText("PainelWorkspace2\\01\\Dados\\maquinas.json")
        ) ?? new();

        return lista.Any(m => m.Ip == ip);
    }
}
