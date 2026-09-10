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
