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

## 2026-09-09 — Endereço vira entidade própria (tabela `endereco`)

**Contexto**: o ERS e `docs/modelo-dados.md` traziam o endereço como colunas
soltas (`rua`, `numero`, `bairro`, `cidade`, `cep`) repetidas em Cliente,
Fornecedor e Funcionário. O Ruan pediu para centralizar.

**Decisão**: criar a tabela/entidade **`endereco`** (`id_endereco` PK + os 5
campos, todos opcionais). Cada cadastro que tem endereço ganha uma FK
**opcional** `id_endereco` (`ON DELETE SET NULL`). Relação **1:1 não
compartilhada** — cada dono tem a sua linha de `endereco`, não se aponta duas
pessoas para o mesmo registro. Migration `002_create_endereco.sql` (cria
`endereco`, adiciona `cliente.id_endereco`, remove as 5 colunas de `cliente`).

**Alternativas consideradas**: manter inline (rejeitado — repetição);
`Endereco` como *value object* com colunas inline (mais leve, sem join, mas
o Ruan quer tabela separada); tabela polimórfica com `owner_type`/`owner_id`
(rejeitado — perde integridade referencial).

**Consequências**: `Fornecedor` e `Funcionario`, quando implementados, seguem
o mesmo padrão (`id_endereco` FK) — `docs/modelo-dados.md` já atualizado. O
`ClienteRepository` grava/atualiza a linha de `endereco` dentro da mesma
transação do cliente; se todos os campos vierem vazios, não cria a linha
(`Endereco.Vazio`). No C#, `Cliente.Endereco` é um `Endereco?`.

## 2026-09-09 — Estória 01 (Clientes): padrões da camada C#

**Contexto**: primeira estória de cadastro implementada em C#; várias escolhas
de estrutura não estavam no ERS e passam a valer como padrão para as próximas.

**Decisões** (todas aprovadas pelo Ruan salvo onde indicado):
- **PF/PJ por composição**, não herança: `Cliente` (raiz) com `Pf` (`ClientePf?`)
  ou `Pj` (`ClientePj?`) e um enum `TipoCliente` mapeado para a string
  "PF"/"PJ" (`TipoClienteExtensions`). Casa com o Dapper (uma raiz para
  carregar/salvar) e evita hierarquia de tipos.
- **Rota REST no plural, minúscula**: `/clientes`, `/clientes/{id}`,
  `/clientes/{id}/inativar`, `/clientes/{id}/reativar`.
- **Resultado de negócio explícito**: `Common/Result` / `Result<T>` + enum
  `TipoFalha` (`Validacao`→400, `NaoEncontrado`→404, `Conflito`→409). Falha de
  RN **não** é exceção — o Service devolve `Result`, o Controller traduz. Só o
  inesperado sobe e cai no `ExceptionHandlingMiddleware`.
- **DTO de request "achatado"** com discriminador `tipo` (um POST/PUT único
  para PF e PJ), endereço como objeto aninhado (`EnderecoRequest`).
  Mapeamento DTO↔domínio manual (sem AutoMapper por ora).
- **Validação de entrada em duas camadas**: `DataAnnotations` nos DTOs para o
  "shape" (obrigatório, tamanho = coluna, e-mail, regex do tipo) — com
  `[ApiController]` isso já retorna `400 ValidationProblemDetails`. Regras que
  dependem de lógica ou de banco ficam no Service. Controller sem `try/catch`.
- **Tipo PF/PJ é imutável na edição** — `AtualizarClienteRequest` não tem
  `tipo`. Trocar de tipo = criar novo cliente e inativar o antigo. (O ERS não
  prevê troca; decisão do agente — vale revisar com o Ruan.)
- **CPF/CNPJ aceitos com máscara e limpos no Service** (persiste só dígitos).
  `[StringLength(14/18)]` no DTO comporta a máscara; a contagem exata de
  dígitos (11/14) é validada no Service (E01 RN03/RN04).
- **Dígito verificador de CPF/CNPJ NÃO é conferido** — o ERS não exige e uma
  checagem parcial daria falsa segurança. Ponto de extensão: `ClienteService.
  ValidarDocumento`. (Decisão do agente — confirmar com o Ruan se deve entrar.)
- **E01 RN05 no `ClienteService`** (não no banco): helpers
  `ExisteAtivoComCpf/CnpjAsync` checam duplicidade **entre clientes ativos**
  ao criar, ao editar cliente ativo e ao reativar → 409.
- **`endereco` órfão pode ser `DELETE`ado**: ao esvaziar o endereço de um
  cliente, o `ClienteRepository` desfaz a FK e apaga a linha de `endereco`.
  É um *value object* sem histórico próprio — não fere a regra de "não
  excluir cadastro" (que continua valendo para `cliente`).

**Consequências**: as Estórias 05 (Fornecedor) e 10 (Funcionário) devem
reaproveitar `Result`, o padrão de rota, a divisão DataAnnotations×Service e a
factory de conexão. **E01 RN01 (perfil de acesso) fica pendente** — sem
autenticação no projeto (rastreado em `ai/plan.md`).
