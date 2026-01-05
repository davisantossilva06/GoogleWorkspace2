using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Middlewares;

public class MiddlewareFiltroMaquina
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<MiddlewareFiltroMaquina> _logger;

    public MiddlewareFiltroMaquina(
        RequestDelegate next,
        ILogger<MiddlewareFiltroMaquina> logger
    )
    {
        _proximo = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext contexto, ServicoMaquinas servicoMaquinas)
    {
        var caminho = contexto.Request.Path.Value?.ToLowerInvariant() ?? "";
        var metodo = contexto.Request.Method;

        _logger.LogInformation(
            "[MiddlewareFiltroMaquina] {Metodo} {Caminho}",
            metodo,
            caminho
        );
        
        if (EhRotaPublica(caminho))
        {
            _logger.LogInformation(
                "[MiddlewareFiltroMaquina] Rota pública liberada: {Caminho}",
                caminho
            );

            await _proximo(contexto);
            return;
        }

        var ip = contexto.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ip))
        {
            _logger.LogWarning(
                "[MiddlewareFiltroMaquina] IP não identificado para {Caminho}",
                caminho
            );

            await ResponderErro(
                contexto,
                StatusCodes.Status403Forbidden,
                "IP da máquina não pôde ser identificado."
            );
            return;
        }

        if (!servicoMaquinas.EstaAutorizada(ip))
        {
            _logger.LogWarning(
                "[MiddlewareFiltroMaquina] Máquina NÃO autorizada | IP: {IP} | Caminho: {Caminho}",
                ip,
                caminho
            );

            await ResponderErro(
                contexto,
                StatusCodes.Status403Forbidden,
                $"Mquina não autorizada (IP: {ip})"
            );
            return;
        }

        _logger.LogInformation(
            "[MiddlewareFiltroMaquina] Máquina autorizada | IP: {IP}",
            ip
        );

        await _proximo(contexto);
    }
    private static bool EhRotaPublica(string caminho)
    {
        return
            caminho == "/" ||
            caminho.StartsWith("/index") ||
            caminho.StartsWith("/login") ||
            caminho.StartsWith("/primeiroacesso") ||
            caminho.StartsWith("/criarsenha") ||
            caminho.StartsWith("/painel") ||
            caminho.StartsWith("/css") ||
            caminho.StartsWith("/js") ||
            caminho.StartsWith("/favicon") ||
            caminho.StartsWith("/lib") ||
            caminho.StartsWith("/logout") ||
            caminho.StartsWith("/_framework");
    }

    private static async Task ResponderErro(
        HttpContext contexto,
        int status,
        string mensagem
    )
    {
        contexto.Response.Clear();
        contexto.Response.StatusCode = status;
        contexto.Response.ContentType = "text/html; charset=utf-8";
        await contexto.Response.WriteAsync($@"
            <html>
                <head>
                    <title>Acesso Negado</title>
                </head>
                <body>
                    <h1>Acesso Negado</h1>
                    <p>{mensagem}</p>
                </body>
            </html>
    
        ");}
}
