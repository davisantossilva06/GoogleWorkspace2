using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Pages;

public class PainelModel : PageModel
{
    private readonly ServicoAutenticacao _autenticacao;
    private readonly ServicoSincronizacao _sincronizacao;

    public PainelModel(ServicoAutenticacao autenticacao, ServicoSincronizacao sincronizacao)
    {
        _autenticacao = autenticacao;
        _sincronizacao = sincronizacao;
    }

    public string EmailUsuario { get; set; } = "";
    public string? TotpProvisionUri { get; set; }
    public string? TotpSecret { get; set; }

    public IActionResult OnGet()
    {
        var email = HttpContext.Session.GetString("usuario_logado");

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Login");
        }

        EmailUsuario = email;
        return Page();
    }

    public IActionResult OnPostHabilitarTotp()
    {
        var email = HttpContext.Session.GetString("usuario_logado");
        if (string.IsNullOrEmpty(email)) return RedirectToPage("/Login");

        var (success, secret, provisionUri) = _autenticacao.HabilitarTotpParaUsuario(email);
        if (!success) return Page();

        TotpSecret = secret;
        TotpProvisionUri = provisionUri;
        return Page();
    }

    public IActionResult OnPostSincronizar()
    {
        var email = HttpContext.Session.GetString("usuario_logado");
        if (string.IsNullOrEmpty(email)) return RedirectToPage("/Login");

        _sincronizacao.SincronizarAgora();
        return Page();
    }
}
