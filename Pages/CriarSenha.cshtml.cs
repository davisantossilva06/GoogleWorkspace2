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

    [BindProperty]
    public string NovaSenha { get; set; } = "";

    [BindProperty]
    public string ConfirmacaoSenha { get; set; } = "";

    public string Erro { get; set; } = "";

    public IActionResult OnGet()
{
    var email = HttpContext.Session.GetString("email_primeiro_acesso");

    if (string.IsNullOrEmpty(email))
    {
        return RedirectToPage("/PrimeiroAcesso");
    }

    return Page();
}
    public IActionResult OnPost()
{
    ModelState.Clear();

    var email = HttpContext.Session.GetString("email_primeiro_acesso");
    if (string.IsNullOrEmpty(email))
    {
        return RedirectToPage("/PrimeiroAcesso");
    }

    var senha = (NovaSenha ?? "").Trim();
    var confirmacao = (ConfirmacaoSenha ?? "").Trim();

    if (senha != confirmacao)
    {
        Erro = "As senhas não coincidem.";
        return Page();
    }

    var usuario = _usuarios.BuscarPorEmail(email);
    if (usuario == null)
    {
        Erro = "Usuário não encontrado.";
        return Page();
    }

    if (usuario.SenhaDefinida)
    {
        return RedirectToPage("/Login");
    }

    _usuarios.DefinirSenha(email, senha);

    HttpContext.Session.Remove("email_primeiro_acesso");
    HttpContext.Session.SetString("usuario_logado", email);

    return RedirectToPage("/painel");
}


}
