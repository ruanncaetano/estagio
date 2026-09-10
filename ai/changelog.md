# Changelog do projeto (visão do agente)

> Log narrativo e datado, em português, do que foi de fato implementado —
> complementa o `git log` com contexto de "por quê", não só "o quê". Uma
> entrada por sessão/dia relevante. Não é changelog de versão pra usuário
> final, é memória de desenvolvimento.

## Formato de entrada
```
## AAAA-MM-DD
- Estória XX implementada: <resumo>.
- Decisões tomadas: <se houver, referenciar docs/decisions.md>.
- Pendências deixadas: <se houver>.
```

---

## 2026-09-09
- **Fase 0 — Estruturação** concluída.
- Repositório git inicializado e publicado em
  `git@github.com:ruanncaetano/estagio.git` (branch `main`).
- Fluxo Git/GitHub definido: branch por bloco, merge `--no-ff`, push em `main`
  ao fechar cada bloco; commits em Conventional Commits pt-BR referenciando
  estória/RN. Detalhe em `docs/conventions.md`.
- Backend reestruturado de template ASP.NET para **Web API por camadas**
  (`Controllers/`, `Models/Domain/`, `Models/Dtos/`, `Services/`,
  `Repositories/`, `Data/`). Removido o `WeatherForecast*` do template;
  adicionado `HealthController` (`GET /health`) como referência da camada.
  Cada pasta tem `README.md` explicando o papel.
- Documentação atualizada: `CLAUDE.md` (raiz + backend), `docs/architecture.md`,
  `docs/conventions.md`, `docs/decisions.md`, `ai/plan.md`, `ai/context.md`.
- Decisões registradas em `docs/decisions.md` (entrada 2026-09-09).
- Pendência deixada: ORM do backend (EF Core vs Dapper) — decidir na Estória 01.

## 2026-09-09 (2)
- **Estória 01 — Gerenciar Clientes**: iniciada pelo banco.
- Decisão: **Dapper + migrations SQL manuais** (MySQL instalado é 5.5.62, sem
  suporte a EF Core moderno). Registrado em `docs/decisions.md`.
- Criado `backend/tcc/tcc/Data/Migrations/` com `000_schema_migration.sql` e
  `001_create_cliente.sql` (+ README da convenção). Aplicados no banco
  `fogo_erp` (MySQL 5.5) e testados: FK barra órfão, charset utf8 com acento,
  join PF/PJ.
- Tabelas: `cliente` (comum, com `tipo_cliente` e `ativo`), `cliente_pf`
  (`cpf`, `rg`), `cliente_pj` (`cnpj`, `nome_fantasia`, `nome_responsavel`,
  `inscricao_estadual`) — herança table-per-type (PK = FK).
- `docs/modelo-dados.md` reconciliado com o ERS: `nome_fantasia` em CLIENTE_PJ;
  nota de que `cpf`/`cnpj` não são `UNIQUE` no banco (RN05 é entre ativos, no Service).
- Pendências deixadas: connection string / pacotes Dapper ainda não
  adicionados; RN05 e demais validações vão na camada Service (próximo bloco).

## 2026-09-09 (3)
- Transversais da API montados (regras novas do Ruan):
  - **Swagger** (Swashbuckle) — `GenerateDocumentationFile` ligado, UI em
    **`/doc`**, JSON em `/swagger/v1/swagger.json`. `HealthController` virou o
    exemplo de referência (`/// <summary>` + `[ProducesResponseType]`).
  - **Serilog** — console + arquivo rotativo em `logs/` (gitignored),
    `UseSerilogRequestLogging`.
  - **`Middleware/ExceptionHandlingMiddleware`** — log de erros central:
    exceção não tratada → `LogError` + `500` `application/problem+json`.
  - Pacotes: Serilog.AspNetCore 10, Serilog.Sinks.File 7, Swashbuckle 10.
- Verificado rodando: `/health` 200, `/doc` 200, swagger.json com o summary
  vindo do XML, rota de erro forçada → 500 + stack trace no arquivo de log.
- Registrado em `docs/decisions.md` (entrada "Transversais da API").
