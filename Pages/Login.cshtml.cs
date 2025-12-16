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
        var usuario = _usuarios.ObterPorEmail(Email);

        if (usuario == null)
        {
            MensagemErro = "Usuário não cadastrado";
            return Page();
        }

        if (!usuario.SenhaDefinida)
        {
            HttpContext.Session.SetString("email_criacao", Email);
            return RedirectToPage("/CriarSenha");
        }

        if (!_usuarios.ValidarSenha(usuario, Senha))
        {
            MensagemErro = "Senha incorreta";
            return Page();
        }

        HttpContext.Session.SetString("usuario_logado", Email);
        return RedirectToPage("/Painel");
    }
}
