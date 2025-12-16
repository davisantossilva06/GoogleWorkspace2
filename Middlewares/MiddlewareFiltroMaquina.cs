namespace SistemaWorkspace.Middlewares;

public class MiddlewareFiltroMaquina
{
    private readonly RequestDelegate proximo;

    public MiddlewareFiltroMaquina(RequestDelegate next)
    {
        proximo = next;
    }

    public async Task Invoke(HttpContext contexto, ServicoMaquinas maquinas)
    {
        var caminho = contexto.Request.Path.Value!.ToLower();

        if (
            caminho.StartsWith("/login") ||
            caminho.StartsWith("/criarsenha") ||
            caminho.StartsWith("/css") ||
            caminho.StartsWith("/js")
        )
        {
            await proximo(contexto);
            return;
        }

        var ip = contexto.Connection.RemoteIpAddress?.ToString() ?? "";

        if (!maquinas.EstaAutorizada(ip))
        {
            contexto.Response.StatusCode = 404;
            return;
        }

        await proximo(contexto);
    }
}
