# Context — onde parei

> Snapshot rápido de handoff. Reescreva isto (não acumule histórico aqui —
> histórico vai pro `ai/changelog.md`) sempre que pausar o trabalho no meio
> de algo.

**Última atualização**: 2026-09-09

**Estava fazendo**: Estória 01 — Gerenciar Clientes. Primeiro bloco (banco)
concluído: tabelas `cliente` / `cliente_pf` / `cliente_pj` + `schema_migration`
criadas no MySQL 5.5 (`fogo_erp`) via `Data/Migrations/000..001`, testadas.
Decidido: Dapper (sem EF Core).

**Próximo passo imediato**: camada C# do Cliente, nesta ordem —
1. `appsettings.Local.json` (ou user-secrets) com a connection string +
   pacote `Dapper` e `MySql.Data`/`MySqlConnector` no `tcc.csproj`.
2. `Models/Domain/`: `Cliente`, `ClientePf`, `ClientePj`.
3. `Models/Dtos/`: `CriarClienteRequest`, `AtualizarClienteRequest`, `ClienteResponse`.
4. `Repositories/`: `IClienteRepository` + `ClienteRepository` (Dapper, sem DELETE).
5. `Services/`: `IClienteService` + `ClienteService` com as RNs (RN05 = unicidade
   entre ativos, validada aqui).
6. `Controllers/ClientesController` + registro de DI no `Program.cs`.

Backlog detalhado em `ai/tasks.md`.

**Bloqueios/dúvidas em aberto**: nenhum. (Atenção: `MySqlConnector` costuma
lidar melhor com MySQL 5.5 que o `MySql.Data` atual — validar ao instalar.)
