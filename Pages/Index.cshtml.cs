using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaWorkspace.Pages;


public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var usuario = HttpContext.Session.GetString("usuario_logado");

        if (string.IsNullOrEmpty(usuario))
        {
            return RedirectToPage("/Login");
        }

        return RedirectToPage("/Painel");
    }
}
