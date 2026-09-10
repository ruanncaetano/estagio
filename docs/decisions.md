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

## 2026-09-09 — Persistência: Dapper + migrations SQL manuais (MySQL 5.5)

**Contexto**: o ambiente do Ruan tem **MySQL 5.5.62** instalado (root/
masterkey). Os providers atuais de EF Core para MySQL (Pomelo 8.x) exigem
5.7+; as versões do Pomelo compatíveis com 5.5 não acompanham o .NET 10.

**Decisão**:
- **Dapper** como camada de acesso a dados (SQL explícito nos Repositories,
  Dapper só materializa o resultado). Resolve a pendência de ORM do `ai/plan.md`.
- Schema versionado como **scripts `.sql` numerados, forward-only**, em
  `backend/tcc/tcc/Data/Migrations/`, controlados por uma tabela
  `schema_migration`. Aplicação manual via cliente `mysql` por enquanto
  (`Data/Migrations/README.md`).
- Banco de dev: `fogo_erp` (charset `utf8`, collation `utf8_unicode_ci`).

**Alternativas consideradas**: EF Core + atualizar o MySQL para 8.0
(descartado agora — não mexer no ambiente do Ruan sem necessidade); EF Core
com Pomelo antigo (incompatível com .NET 10).

**Consequências**:
- No 5.5 não há `utf8mb4` viável para índices longos (limite de 767 bytes),
  CHECK constraint nem coluna gerada. Invariantes que dependeriam disso vão
  para o `ClienteService`.
- **RN05** (nenhum par de clientes *ativos* com o mesmo CPF/CNPJ) é validada
  no Service, **não** como `UNIQUE` — um UNIQUE simples barraria também
  dois inativos, ou um ativo + um inativo, com o mesmo documento (o ERS
  permite). Índices não-únicos em `cpf`/`cnpj` só aceleram a consulta.
- `docs/modelo-dados.md` (CLIENTE_PJ) reconciliado com o ERS: `nome_fantasia`
  adicionado (estava em `comercial.md`, faltava no modelo); `inscricao_estadual`
  mantido como opcional.

## 2026-09-09 — Transversais da API: Swagger obrigatório + Serilog

**Contexto**: regras de implementação pedidas pelo Ruan antes de subir os
primeiros endpoints — a API precisa ser autodocumentada, testável por uma UI,
e ter log de erros.

**Decisão**:
- **Swashbuckle.AspNetCore**. `GenerateDocumentationFile` ligado no
  `tcc.csproj`; `AddSwaggerGen` consome o XML. Todo endpoint com
  `/// <summary>` + `[ProducesResponseType]` (regra em `backend/CLAUDE.md`).
  UI em **`/doc`** (`RoutePrefix = "doc"`), JSON em `/swagger/v1/swagger.json`.
  Ligado em todos os ambientes.
- **Serilog** (`Serilog.AspNetCore` + `Serilog.Sinks.File`). Console +
  arquivo rotativo diário em `logs/` (14 dias), nível em `appsettings.json`
  seção `Serilog`. `UseSerilogRequestLogging` para uma linha por request.
- **`Middleware/ExceptionHandlingMiddleware`** — captura exceção não tratada,
  `LogError` com stack trace/rota/traceId, responde `500`
  `application/problem+json` sem vazar detalhe. Falha de RN não passa por
  aqui: o Service devolve resultado de falha e o Controller traduz p/ 4xx.

**Alternativas consideradas**: `Microsoft.AspNetCore.OpenApi` puro (novo
padrão dos templates .NET 9+) — não traz UI; o Ruan quer o Swagger UI.
`AddProblemDetails` + `IExceptionHandler` — equivalente; middleware explícito
é mais didático.

**Consequências**: 1591 (membro público sem doc XML) fica como aviso
silenciado — não quebra build, mas documentar é regra. `logs/` e `*.log`
no `.gitignore`. Pacotes novos no `tcc.csproj`: Serilog.AspNetCore,
Serilog.Sinks.File, Swashbuckle.AspNetCore.
