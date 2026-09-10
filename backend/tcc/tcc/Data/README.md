# Data — conexão e persistência (MySQL 5.5 + Dapper)

Configuração da conexão com o MySQL e a infra que os Repositories usam.

**Decisão (2026-09-09 — `docs/decisions.md`)**: acesso a dados com **Dapper**
(SQL explícito no Repository, Dapper só materializa o resultado). Sem EF Core
— MySQL 5.5 não é suportado pelos providers atuais.

## Schema
`Migrations/` — scripts `.sql` numerados, forward-only, aplicados na mão via
cliente `mysql` e controlados pela tabela `schema_migration`. Ver
`Migrations/README.md`. Banco de dev: `fogo_erp`.

## Connection string
Lida via `IConfiguration`, **não versionada** — usar `dotnet user-secrets`
ou `appsettings.Local.json` (coberto pelo `.gitignore`). O
`appsettings.Development.json` versionado só carrega config não-sensível.

Formato: `Server=localhost;Port=3306;Database=fogo_erp;Uid=root;Pwd=***;`
