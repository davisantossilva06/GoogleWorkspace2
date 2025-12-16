namespace SistemaWorkspace.Modelos;

public class UsuarioSistema
{
    public string Email { get; set; } = "";
    public bool SenhaDefinida { get; set; }
    public string Salt { get; set; } = "";
    public string HashSenha { get; set; } = "";
}
