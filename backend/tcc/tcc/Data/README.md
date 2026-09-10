# Data — conexão e contexto de persistência (MySQL)

Configuração da conexão com o MySQL e o contexto/infra que os Repositories
usam.

> **Decisão em aberto**: ORM não escolhido. EF Core (migrations, mais
> produtivo, mais "mágica") vs Dapper (SQL explícito, mais didático pra
> aprender C#/SQL). A decidir na **Estória 01 — Gerenciar Clientes**.
> Registrado como pendência em `ai/plan.md`.

Enquanto não decidir: string de conexão fica em `appsettings.Development.json`
(local, fora do git) e é lida via `IConfiguration`.
