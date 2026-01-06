# SistemaWorkspace — Resumo de alterações

Este fork contém correções e melhorias iniciais para tornar o sistema funcional conforme solicitado.

Principais mudanças:

- Atualizado `global.json` para usar SDK disponível no ambiente de desenvolvimento.
- Corrigido uso obsoleto da API do Google (ServiceAccountCredential -> ToGoogleCredential).
- Melhorias na autenticação local:
  - Uso de PBKDF2 (KeyDerivation.Pbkdf2) com salt único por usuário.
  - Suporte a TOTP (Google Authenticator) via `Otp.NET`.
- Implementado fluxo de habilitação de 2FA na interface `/Painel` (botão para habilitar e QR code).
- Implementado sincronização com Google Workspace (`ServicoSincronizacao`) que grava cache em `Dados/usuarios_workspace.json`.
- Adicionadas variáveis de ambiente para segurança:
  - `GOOGLE_SERVICE_ACCOUNT_PATH` — caminho para o JSON de conta de serviço.
  - `GOOGLE_ADMIN_EMAIL` — email do administrador a ser usado em impersonation.

Avisos pendentes:
- Dependência `System.Text.Json` está com alerta de vulnerabilidade (ex.: `NU1903`) — recomenda-se atualizar para uma versão segura assim que disponível.

Próximos passos sugeridos:
- Migrar armazenamento de usuários para um banco de dados (SQLite/EF Core) para produção.
- Implementar testes automatizados para fluxos de login, TOTP e sincronização.
- Remover `Dados/google_service_account.json` do repositório e armazenar em secrets manager.

---

Se quiser, eu prossigo agora com:
1) Migrar `usuarios.json` para EF Core + SQLite e criar as migrations; ou
2) Implementar endpoints/API para exibir os dados sincronizados; ou
3) Ajustar o layout da UI e adicionar testes e CI.

Diga qual a prioridade.