using OtpNet;
using SistemaWorkspace.Modelos;

namespace SistemaWorkspace.Servicos;

public class ServicoAutenticacao
{
    private readonly ServicoUsuarios _servicoUsuarios;
    private readonly ServicoSincronizacao _sincronizacao;
    private readonly string _issuer = "SistemaWorkspace";

    public ServicoAutenticacao(ServicoUsuarios servicoUsuarios, ServicoSincronizacao sincronizacao)
    {
        _servicoUsuarios = servicoUsuarios;
        _sincronizacao = sincronizacao;
    }

    public string GerarTotpSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GetProvisionUri(string email, string secret)
    {
        // otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}&digits=6
        var escEmail = Uri.EscapeDataString(email);
        return $"otpauth://totp/{_issuer}:{escEmail}?secret={secret}&issuer={_issuer}&digits=6";
    }

    public bool VerificarCodigo(string? secret, string codigo)
    {
        if (string.IsNullOrEmpty(secret)) return false;

        var key = Base32Encoding.ToBytes(secret);
        var totp = new Totp(key, step:30, totpSize:6);
        return totp.VerifyTotp(codigo, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }

    public (bool success, string? secret, string? provisionUri) HabilitarTotpParaUsuario(string email)
    {
        var usuario = _servicoUsuarios.BuscarPorEmail(email);
        if (usuario == null) return (false, null, null);

        var secret = GerarTotpSecret();
        usuario.TotpSecret = secret;
        usuario.TotpEnabled = true;

        // Salvar alteração
        _servicoUsuarios.AtualizarUsuario(usuario);

        // Iniciar sincronização imediata após habilitar 2FA
        try
        {
            _sincronizacao.SincronizarAgora();
        }
        catch { /* não falhar o fluxo de habilitação */ }

        var provision = GetProvisionUri(usuario.Email, secret);
        return (true, secret, provision);
    }

    public bool VerificarTotpUsuario(string email, string codigo)
    {
        var usuario = _servicoUsuarios.BuscarPorEmail(email);
        if (usuario == null || !usuario.TotpEnabled) return false;

        return VerificarCodigo(usuario.TotpSecret, codigo);
    }
}
