using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace SistemaWorkspace.Servicos
{
    public class ServicoGoogleWorkspace
    {
        private readonly string _caminhoCredenciais;
        private readonly string _emailAdministrador;

        private static readonly string[] Escopos =
        {
            DirectoryService.Scope.AdminDirectoryUserReadonly,
            DirectoryService.Scope.AdminDirectoryGroupReadonly
        };

        public ServicoGoogleWorkspace(IWebHostEnvironment ambiente)
        {
            _caminhoCredenciais = Path.Combine(
                ambiente.ContentRootPath,
                "Dados",
                "google_service_account.json"
            );

            if (!File.Exists(_caminhoCredenciais))
                throw new FileNotFoundException(
                    "Arquivo google_service_account.json não encontrado em /Dados"
                );

            _emailAdministrador = LerEmailAdministrador();
        }
        private DirectoryService CriarDirectoryService()
        {
            GoogleCredential credencial;

            using (var stream = new FileStream(_caminhoCredenciais, FileMode.Open, FileAccess.Read))
            {
                credencial = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(Escopos)
                    .CreateWithUser(_emailAdministrador);
            }

            return new DirectoryService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credencial,
                    ApplicationName = "SistemaWorkspace"
                }
            );
        }

        public List<UsuarioWorkspace> ListarUsuarios()
        {
            var servico = CriarDirectoryService();
            var listaUsuarios = new List<UsuarioWorkspace>();

            var requisicao = servico.Users.List();
            requisicao.Customer = "my_customer";
            requisicao.MaxResults = 500;
            requisicao.OrderBy = UsersResource.ListRequest.OrderByEnum.Email;

            do
            {
                Users resposta;

                try
                {
                    resposta = requisicao.Execute();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao consultar usuários do Google Workspace", ex);
                }

                if (resposta.UsersValue != null)
                {
                    foreach (var usuario in resposta.UsersValue)
                    {
                        listaUsuarios.Add(new UsuarioWorkspace
                        {
                            Email = usuario.PrimaryEmail ?? "",
                            NomeCompleto = usuario.Name?.FullName ?? "",
                            Ativo = usuario.Suspended == false,
                        });
                    }
                }

                requisicao.PageToken = resposta.NextPageToken;

            } while (!string.IsNullOrEmpty(requisicao.PageToken));

            return listaUsuarios;
        }
        public List<CaixaEmailWorkspace> ListarCaixasEmail()
        {
            var usuarios = ListarUsuarios();

            return usuarios.Select(u => new CaixaEmailWorkspace
            {
                Email = u.Email,
                Ativa = u.Ativo,
                UltimoAcesso = u.UltimoLogin
            }).ToList();
        }
        public ResumoWorkspace ObterResumo()
        {
            var usuarios = ListarUsuarios();

            return new ResumoWorkspace
            {
                TotalUsuarios = usuarios.Count,
                UsuariosAtivos = usuarios.Count(u => u.Ativo),
                UsuariosSuspensos = usuarios.Count(u => !u.Ativo),
                UltimoLoginGeral = usuarios
                    .Where(u => !string.IsNullOrEmpty(u.UltimoLogin))
                    .Max(u => u.UltimoLogin)
            };
        }

        private string LerEmailAdministrador()
        {
            var json = File.ReadAllText(_caminhoCredenciais);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("client_email", out _))
                throw new Exception("Arquivo google_service_account.json inválido");

            return "admin@painelworkspace.com";
        }
    }

    public class UsuarioWorkspace
    {
        public string Email { get; set; } = "";
        public string NomeCompleto { get; set; } = "";
        public bool Ativo { get; set; }
        public string? UltimoLogin { get; set; }
    }

    public class CaixaEmailWorkspace
    {
        public string Email { get; set; } = "";
        public bool Ativa { get; set; }
        public string? UltimoAcesso { get; set; }
    }

    public class ResumoWorkspace
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosAtivos { get; set; }
        public int UsuariosSuspensos { get; set; }
        public string? UltimoLoginGeral { get; set; }
    }
}
