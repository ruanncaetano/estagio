# Data — conexão e contexto de persistência (MySQL)

Configuração da conexão com o MySQL e o contexto/infra que os Repositories
usam.

> **Decisão em aberto**: ORM não escolhido. EF Core (migrations, mais
> produtivo, mais "mágica") vs Dapper (SQL explícito, mais didático pra
> aprender C#/SQL). A decidir na **Estória 01 — Gerenciar Clientes**.
> Registrado como pendência em `ai/plan.md`.

Enquanto não decidir: a string de conexão é lida via `IConfiguration`, mas
**não vai versionada** — usar `dotnet user-secrets` ou um
`appsettings.Local.json` (já coberto pelo `.gitignore`). O
`appsettings.Development.json` versionado só carrega config não-sensível
(logging etc.).
