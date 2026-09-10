# Decisões — log cronológico

> Registro de decisões técnicas/de produto que não estavam (ou não estavam
> claras) no ERS original, com data e motivo. `docs/architecture.md`
> descreve o estado atual; aqui fica o "por quê chegamos nele" e o que foi
> descartado no caminho.

## Formato de entrada
```
## AAAA-MM-DD — <título curto da decisão>
**Contexto**: por que essa decisão precisou ser tomada.
**Decisão**: o que foi decidido.
**Alternativas consideradas**: (se houver)
**Consequências**: o que isso implica pro resto do sistema.
```

---

## 2026-09-09 — Estrutura de documentação e memória do agente

**Contexto**: início do projeto de implementação do ERP a partir do ERS
pronto; precisava de uma estrutura de arquivos pra guiar o Claude Code.

**Decisão**: separar `docs/` (verdade estável do produto: requisitos,
arquitetura, convenções, modelo de dados, glossário) de `ai/` (estado de
trabalho do agente: plano, tasks, contexto de sessão, changelog). Requisitos
do ERS quebrados por módulo de negócio (Comercial, Produção,
Operação/Eventos, Financeiro, Relatórios/Dashboard) em vez de um único
arquivo ou um arquivo por estória.

**Alternativas consideradas**: um arquivo por estória (25 arquivos — achado
granular demais); tudo em um `requirements.md` único (achado grande demais
pra navegar); pasta `adr/` com um arquivo por decisão de arquitetura
(trocado por este log único, mais leve pro tamanho do projeto).

**Consequências**: `CLAUDE.md` fica enxuto e aponta pra esses arquivos em
vez de conter tudo inline. Backend e frontend mantêm `CLAUDE.md` próprios
dentro de suas pastas (não centralizados em `docs/conventions.md`), por
preferência do time.

## 2026-09-09 — Estrutura de camadas do backend + fluxo Git/GitHub

**Contexto**: antes de começar a Estória 01, o backend ainda era o template
`tcc` da ASP.NET (sem camadas) e o projeto não estava em git. Precisava de
uma estrutura de pastas definida e de um fluxo de versionamento antes de
escrever qualquer código de negócio.

**Decisão**:
1. **Backend em Web API por camadas** — projeto único `backend/tcc/tcc/` com
   `Controllers/` (HTTP), `Models/Domain/` (entidades), `Models/Dtos/`
   (contratos de API), `Services/` (regra de negócio + validações de RN),
   `Repositories/` (acesso a MySQL), `Data/` (conexão). Interface +
   implementação por módulo, ligadas por DI no `Program.cs`.
2. **Fluxo Git**: repositório `git@github.com:ruanncaetano/estagio.git`;
   `main` sempre entregável; uma branch por bloco (`feat/estoria-XX-…`,
   `chore/…`, `docs/…`); merge com `--no-ff` e `git push origin main` ao
   fechar cada bloco. Commits em Conventional Commits pt-BR referenciando
   estória/RN. Detalhe em `docs/conventions.md`.

**Alternativas consideradas**:
- *Vertical Slice* (pasta por feature em vez de por camada) — descartado por
  ser menos familiar para quem está aprendendo C#; camadas explícitas
  ensinam melhor a separação de responsabilidades.
- *ASP.NET MVC clássico com Views Razor* — descartado por contrariar o stack
  (frontend é React/Next.js separado).
- *Renomear projeto `tcc` → `FogoErp.Api`* — adiado; churn sem ganho agora.
- *Trunk-based sem branches* — descartado; o Ruan quer praticar branch/merge.

**Consequências**:
- `backend/CLAUDE.md` e `docs/architecture.md` passam a descrever as camadas
  como decisão vigente (não mais "A definir").
- A escolha do ORM (EF Core vs Dapper) fica pendente para a Estória 01 —
  registrada em `ai/plan.md`.
- Toda entrega passa a exigir commit em branch + merge em `main` + push.
