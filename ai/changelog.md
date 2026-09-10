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

## 2026-09-09 (4)
- **Estória 01 — Gerenciar Clientes: camada C# completa** (backend). Fluxo
  `Controller → Service (RNs) → Repository (Dapper) → MySQL` funcionando ponta
  a ponta contra o banco `fogo_erp`.
- Schema: migration `002_create_endereco.sql` — endereço extraído para tabela
  própria `endereco`, `cliente.id_endereco` FK opcional (`ON DELETE SET NULL`).
  Aplicada no `fogo_erp`.
- Novos arquivos:
  - Domínio: `Cliente`, `ClientePf`, `ClientePj`, `Endereco`, `TipoCliente`
    (enum + `TipoClienteExtensions` "PF"/"PJ").
  - DTOs: `CriarClienteRequest`, `AtualizarClienteRequest`, `ClienteResponse`,
    `EnderecoRequest`, `EnderecoResponse`.
  - `Common/Result.cs` — `Result` / `Result<T>` + enum `TipoFalha`
    (Validacao→400, NaoEncontrado→404, Conflito→409). Falha de RN não é exceção.
  - `Data/IDbConnectionFactory` + `MySqlConnectionFactory` (Dapper +
    `MatchNamesWithUnderscores`). `appsettings.Local.json` (gitignored) +
    `appsettings.Local.example.json` (versionado, senha mascarada).
  - `Repositories/IClienteRepository` + `ClienteRepository` — SELECT com
    `LEFT JOIN` PF/PJ/endereco, INSERT/UPDATE multi-tabela em transação,
    helpers de unicidade para a E01 RN05. Sem `DELETE` de cliente.
  - `Services/IClienteService` + `ClienteService` — validações E01 RN02/RN03/
    RN04/RN05/RN06/RN07/RN08, cada uma com comentário `// E01 RNxx`.
  - `Controllers/ClientesController` — `GET /clientes` (filtros `ativo`,
    `busca`), `GET /clientes/{id}`, `POST`, `PUT /{id}`,
    `PATCH /{id}/inativar`, `PATCH /{id}/reativar`. `/// <summary>` +
    `[ProducesResponseType]` em todos.
- Validação de entrada: `DataAnnotations` nos request DTOs (shape: obrigatório,
  tamanho, e-mail, regex do tipo). Com `[ApiController]`, viola → `400
  ValidationProblemDetails` automático. Controller magro, sem try/catch (o
  `ExceptionHandlingMiddleware` cobre o inesperado). Regras de lógica/banco
  (contagem de dígitos do CPF/CNPJ, unicidade E01 RN05) ficam no Service.
- Pacotes adicionados: `Dapper` 2.1.79, `MySqlConnector` 2.6.2.
- `Program.cs`: carrega `appsettings.Local.json`; registra
  `IDbConnectionFactory`, `IClienteRepository`, `IClienteService` (scoped).
- Decisões do agente (registradas em `docs/decisions.md`, entrada
  "Estória 01 — camada C#"): rota `/clientes` (plural); DTO de request
  "achatado" com discriminador `tipo`; tipo PF/PJ imutável na edição;
  documento aceito com máscara e limpo no Service (guarda só dígitos);
  **dígito verificador de CPF/CNPJ não conferido** (ERS não exige);
  endereço órfão (value-object sem dono) pode ser `DELETE`ado — não é cadastro.
- Verificado com curl contra `fogo_erp`: criar PF com endereço → 201; criar PJ
  sem endereço → 201; 2º cliente ativo com mesmo CPF → 409 (E01 RN05);
  inativar → 200; novo cliente ativo reusando o CPF → 201; reativar o antigo
  (agora colidindo) → 409; `GET ?ativo=true|false`, `GET ?busca=` (nome e
  documento) OK; `GET /{id}` inexistente → 404; DataAnnotations (`tipo`
  ausente, e-mail inválido) → 400 `ValidationProblemDetails`; CPF com 12
  dígitos → 400 `codigo=E01_RN03`; `/doc` 200 e lista os 4 caminhos de
  `/clientes`. Dados de teste removidos do banco.
- Pendência deixada: **E01 RN01** (perfil de acesso) — sem auth no projeto;
  `// E01 RN01 — TODO` no Service; registrada em `ai/plan.md`.

## 2026-09-09 (5)
- Documentação da API: trocado **Swashbuckle → `Microsoft.AspNetCore.OpenApi`**
  (10.0.12) + **`Scalar.AspNetCore`** (2.17.3) para a UI. Decisão do Ruan.
  - `Program.cs`: `AddOpenApi` (título/descrição via `AddDocumentTransformer`)
    + `MapOpenApi` + `MapScalarApiReference("/doc", ...)`.
  - JSON agora em **`/openapi/v1.json`** (era `/swagger/v1/swagger.json`); UI
    de teste segue em **`/doc`** (Scalar, com "try it out").
  - Comentários `///` continuam alimentando a doc — no .NET 10 o source
    generator do pacote lê o XML automaticamente.
  - Verificado: build verde; `/openapi/v1.json` 200 com os summaries dos
    endpoints de `/clientes` vindos do XML; `/doc` 200 (Scalar).
- `backend/CLAUDE.md`, `docs/architecture.md`, `docs/decisions.md` atualizados.
