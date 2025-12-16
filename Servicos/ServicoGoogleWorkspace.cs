using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Text.Json;

namespace SistemaWorkspace.Servicos
{
    public class ServicoGoogleWorkspace
    {
        
        private readonly string _caminhoCredenciais;
        private readonly string _emailAdministrador;

        private readonly string[] _escopos =
        {
            DirectoryService.Scope.AdminDirectoryUserReadonly,
            DirectoryService.Scope.AdminDirectoryGroupReadonly
        };

        public ServicoGoogleWorkspace(IWebHostEnvironment env)
        {
            _caminhoCredenciais = Path.Combine(
                env.ContentRootPath,
                "Dados",
                "google_auth.json"
            );

            _emailAdministrador = LerEmailAdministrador();
        }

        private DirectoryService CriarServicoDirectory()
        {
            if (!File.Exists(_caminhoCredenciais))
                throw new FileNotFoundException("Arquivo google_auth.json não encontrado");

            GoogleCredential credencial;

            using (var stream = new FileStream(_caminhoCredenciais, FileMode.Open, FileAccess.Read))
            {
                credencial = GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(_escopos)
                    .CreateWithUser(_emailAdministrador);
            }

            return new DirectoryService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credencial,
                ApplicationName = "SistemaWorkspace"
            });
        }

        public List<UsuarioWorkspace> ListarUsuarios()
        {
            var service = CriarServicoDirectory();
            var usuarios = new List<UsuarioWorkspace>();

            var requisicao = service.Users.List();
            requisicao.Customer = "my_customer";
            requisicao.MaxResults = 500;
            requisicao.OrderBy = UsersResource.ListRequest.OrderByEnum.Email;

            do
            {
                var resposta = requisicao.Execute();

                if (resposta.UsersValue != null)
                {
                    foreach (var u in resposta.UsersValue)
                    {
                        usuarios.Add(new UsuarioWorkspace
                        {
                            Email = u.PrimaryEmail,
                            NomeCompleto = u.Name.FullName,
                            Ativo = !u.Suspended,
                            UltimoLogin = u.LastLoginTime
                        });
                    }
                }

                requisicao.PageToken = resposta.NextPageToken;

            } while (!string.IsNullOrEmpty(requisicao.PageToken));

            return usuarios;
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

            if (!doc.RootElement.TryGetProperty("admin_email", out var email))
                throw new Exception("admin_email não encontrado no google_auth.json");

            return email.GetString()!;
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
