using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Pages;

public class PrimeiroAcessoModel : PageModel
{
    private readonly ServicoUsuarios _usuarios;

    public PrimeiroAcessoModel(ServicoUsuarios usuarios)
    {
        _usuarios = usuarios;
    }

    [BindProperty]
    public string Email { get; set; } = "";

    public string Erro { get; set; } = "";

    public IActionResult OnPost()
{
    if (string.IsNullOrWhiteSpace(Email))
    {
        Erro = "Informe o email.";
        return Page();
    }

    var email = Email.Trim().ToLower();
    var usuario = _usuarios.BuscarPorEmail(email);

    if (usuario == null)
    {
        Erro = "Este email não existe no sistema.";
        return Page();
    }

    if (usuario.SenhaDefinida)
    {
        Erro = "Este usuário já possui senha. Faça login.";
        return Page();
    }

    HttpContext.Session.SetString("email_primeiro_acesso", email);

    return RedirectToPage("/CriarSenha");
}

}
