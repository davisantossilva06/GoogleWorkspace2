using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace SistemaWorkspace.Pages;

public class PainelModel : PageModel
{
    public string EmailUsuario { get; set; } = "";

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
}
