# Context — onde parei

> Snapshot rápido de handoff. Reescreva isto (não acumule histórico aqui —
> histórico vai pro `ai/changelog.md`) sempre que pausar o trabalho no meio
> de algo.

**Última atualização**: 2026-09-09 (5)

**Estava fazendo**: Estória 01 — Gerenciar Clientes, camada C# (backend).
Depois: troca da doc da API para `Microsoft.AspNetCore.OpenApi` + Scalar
(UI em `/doc`, JSON em `/openapi/v1.json`) — ver `ai/changelog.md` (5).

**Concluído nesta sessão**:
- Banco: migration `002_create_endereco.sql` aplicada — endereço virou tabela
  própria (`endereco`), `cliente.id_endereco` FK opcional (`ON DELETE SET NULL`).
- Backend C# completo do módulo Cliente:
  - `Models/Domain/`: `Cliente` (composição com `Pf`/`Pj`/`Endereco`),
    `ClientePf`, `ClientePj`, `Endereco`, enum `TipoCliente` + `TipoClienteExtensions`.
  - `Models/Dtos/`: `CriarClienteRequest`, `AtualizarClienteRequest`,
    `ClienteResponse`, `EnderecoRequest`, `EnderecoResponse`. DataAnnotations
    nos requests (shape); regras de RN no Service.
  - `Common/Result.cs`: `Result` / `Result<T>` + `TipoFalha`.
  - `Data/`: `IDbConnectionFactory` + `MySqlConnectionFactory` (lê
    `ConnectionStrings:MySql`). `appsettings.Local.json` (gitignored) +
    `appsettings.Local.example.json` (versionado). `Program.cs` carrega o Local.
  - `Repositories/`: `IClienteRepository` + `ClienteRepository` (Dapper,
    escrita multi-tabela em transação, sem DELETE de cliente).
  - `Services/`: `IClienteService` + `ClienteService` — RN02/RN03/RN04/RN05/
    RN06/RN07/RN08 com comentário `// E01 RNxx`. RN01 = TODO (sem auth).
  - `Controllers/ClientesController` — GET lista (`ativo`, `busca`), GET/{id},
    POST, PUT/{id}, PATCH/{id}/inativar, PATCH/{id}/reativar. Rota `/clientes`.
  - DI registrado no `Program.cs`.
- Pacotes: `Dapper` 2.1.79, `MySqlConnector` 2.6.2.
- `dotnet build tcc.slnx` verde (0 warning / 0 erro).
- Testado com curl contra `fogo_erp` (PF/PJ 201, RN05 409, inativar→reusar CPF
  201, reativar colidindo 409, filtros, busca, 404, DataAnnotations 400,
  `/doc`). Dados de teste removidos; tabelas vazias, AUTO_INCREMENT resetado.

**Próximo passo imediato**: frontend da Estória 01 — tela de listagem (cards
Total/Ativos/Inativos/Exibindo + busca + filtro + exportar) e modal de
cadastro/edição com abas, alternando campos PF x PJ. Ver `ai/tasks.md`.

**Pendências desta estória**:
- **E01 RN01** (acesso só Administrador / Vendedor-Comercial) não implementada
  — não há autenticação no projeto. `// E01 RN01 — TODO` no `ClienteService`.
  Registrada em `ai/plan.md`.
