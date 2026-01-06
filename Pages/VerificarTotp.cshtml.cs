using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Pages;

public class VerificarTotpModel : PageModel
{
    private readonly ServicoAutenticacao _autenticacao;

    public VerificarTotpModel(ServicoAutenticacao autenticacao)
    {
        _autenticacao = autenticacao;
    }

    [BindProperty]
    public string Codigo { get; set; } = "";

    public string MensagemErro { get; set; } = "";

    public IActionResult OnGet()
    {
        var temp = HttpContext.Session.GetString("usuario_temporario");
        if (string.IsNullOrEmpty(temp)) return RedirectToPage("/Login");
        return Page();
    }

    public IActionResult OnPost()
    {
        var temp = HttpContext.Session.GetString("usuario_temporario");
        if (string.IsNullOrEmpty(temp)) return RedirectToPage("/Login");

        if (_autenticacao.VerificarTotpUsuario(temp, Codigo.Trim()))
        {
            HttpContext.Session.Remove("usuario_temporario");
            HttpContext.Session.SetString("usuario_logado", temp);
            return RedirectToPage("/Painel");
        }

        MensagemErro = "Código inválido.";
        return Page();
    }
}