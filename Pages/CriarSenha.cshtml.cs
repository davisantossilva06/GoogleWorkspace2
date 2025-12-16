using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Pages;

public class CriarSenhaModel : PageModel
{
    private readonly ServicoUsuarios _usuarios;

    public CriarSenhaModel(ServicoUsuarios usuarios)
    {
        _usuarios = usuarios;
    }

    [BindProperty] public string NovaSenha { get; set; } = "";
    [BindProperty] public string ConfirmacaoSenha { get; set; } = "";

    public string MensagemErro { get; set; } = "";

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("email_criacao") == null)
            return RedirectToPage("/Login");

        return Page();
    }

    public IActionResult OnPost()
    {
        var email = HttpContext.Session.GetString("email_criacao");
        if (email == null) return RedirectToPage("/Login");

        if (NovaSenha.Length < 8)
        {
            MensagemErro = "A senha deve ter no mínimo 8 caracteres";
            return Page();
        }

        if (NovaSenha != ConfirmacaoSenha)
        {
            MensagemErro = "As senhas não coincidem";
            return Page();
        }

        var usuario = _usuarios.ObterPorEmail(email);
        if (usuario == null) return RedirectToPage("/Login");

        _usuarios.DefinirSenha(usuario, NovaSenha);

        HttpContext.Session.Remove("email_criacao");
        HttpContext.Session.SetString("usuario_logado", email);

        return RedirectToPage("/Painel");
    }
}
