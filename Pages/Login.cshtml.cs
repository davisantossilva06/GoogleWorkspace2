using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaWorkspace.Servicos;

namespace SistemaWorkspace.Pages;

public class LoginModel : PageModel
{
    private readonly ServicoUsuarios _usuarios;

    public LoginModel(ServicoUsuarios usuarios)
    {
        _usuarios = usuarios;
    }

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Senha { get; set; } = "";

    public string MensagemErro { get; set; } = "";

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            MensagemErro = "Informe email e senha.";
            return Page();
        }

        var email = Email.Trim().ToLower();
        var usuario = _usuarios.BuscarPorEmail(email);

        if (usuario == null)
        {
            MensagemErro = "Email não encontrado.";
            return Page();
        }

        if (!usuario.SenhaDefinida)
        {
            HttpContext.Session.SetString("email_primeiro_acesso", email);
            return RedirectToPage("/CriarSenha");
        }

        if (!_usuarios.ValidarSenha(email, Senha))
        {
            MensagemErro = "Senha incorreta.";
            return Page();
        }

        HttpContext.Session.SetString("usuario_logado", email);
        return RedirectToPage("/Painel");
    }
}
