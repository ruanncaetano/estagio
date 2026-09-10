# Tasks — backlog granular

> Diferença pro `ai/plan.md`: o plan.md marca status por **estória inteira**.
> Aqui quebramos a estória atual (ou as próximas) em passos técnicos
> concretos, que sobrevivem entre sessões (diferente do TodoWrite, que é só
> da sessão corrente). Quando uma estória é concluída, apague as tasks dela
> daqui e marque o checkbox correspondente no `ai/plan.md`.

## Convenção de uso

Ao iniciar uma estória nova, quebrar em algo como:
```
## Estória XX — Nome
- [ ] Migration da tabela X
- [ ] Entidade/DTO em C#
- [ ] Endpoint(s) — método/rota
- [ ] Validações de RN: RN01, RN03, RN05...
- [ ] Tela de listagem
- [ ] Modal/form de cadastro-edição
- [ ] Teste manual do fluxo completo
```

Task só sai daqui quando estiver de fato pronta (não só "código escrito" —
também validada contra as RNs do módulo em `docs/requisitos/`).

## Fila atual

### Estória 01 — Gerenciar Clientes  (`docs/requisitos/comercial.md`)

Branch sugerida: `feat/estoria-01-clientes`.

**Decisão tomada (2026-09-09)**: Dapper + migrations SQL manuais (MySQL 5.5).
Ver `docs/decisions.md`.

- [x] ~~Definir ORM~~ → Dapper. Banco dev `fogo_erp` criado.
- [x] Configurar conexão MySQL: `appsettings.Local.json` (gitignored) +
      `appsettings.Local.example.json` (versionado) + `IDbConnectionFactory`
      (`MySqlConnectionFactory`), lida via `IConfiguration.GetConnectionString("MySql")`.
      `Program.cs` carrega `appsettings.Local.json` (optional).
- [x] Migration/DDL das tabelas: `cliente`, `cliente_pf`, `cliente_pj` +
      `schema_migration` — `Data/Migrations/000..001` aplicados e testados
      (FK, charset utf8, join PF/PJ)
- [x] Migration `002_create_endereco.sql` — endereço extraído para tabela
      própria (`endereco`, FK opcional `cliente.id_endereco`). Aplicada.
- [x] `Models/Domain/`: `Cliente` (composição), `ClientePf`, `ClientePj`,
      `Endereco`, enum `TipoCliente` (+ `TipoClienteExtensions` "PF"/"PJ")
- [x] `Models/Dtos/`: `CriarClienteRequest`, `AtualizarClienteRequest`,
      `ClienteResponse`, `EnderecoRequest`, `EnderecoResponse` (endereço aninhado)
- [x] `Common/Result.cs`: `Result` / `Result<T>` + enum `TipoFalha`
      (Validacao→400, NaoEncontrado→404, Conflito→409)
- [x] Validação de entrada: `DataAnnotations` nos request DTOs (`[Required]`,
      `[StringLength]`, `[EmailAddress]`, `[RegularExpression]` no tipo).
      `[ApiController]` devolve `400 ValidationProblemDetails` automático;
      controller magro, sem try/catch (middleware cobre o inesperado).
      Regras de lógica/banco (contagem de dígitos, E01 RN05) seguem no Service.
- [x] `Repositories/`: `IClienteRepository` + `ClienteRepository` (Dapper,
      sem DELETE de cliente — RN06; escrita multi-tabela em transação)
- [x] `Services/`: `IClienteService` + `ClienteService` com as validações de RN:
  - [ ] RN01 — acesso só Administrador e Vendedor/Comercial → **PENDENTE** (sem
        auth no projeto; `// E01 RN01 — TODO` no Service; ver `ai/plan.md`)
  - [x] RN02 — aceitar PF e PJ
  - [x] RN03 / RN04 — CPF (PF) e CNPJ (PJ) opcionais; quando informados,
        11/14 dígitos (dígito verificador NÃO checado — ERS não exige)
  - [x] RN05 — bloqueia dois clientes **ativos** com mesmo CPF/CNPJ → 409
        (criar, editar cliente ativo, e reativar)
  - [x] RN06 / RN07 — inativação/reativação (`ativo`), nunca exclusão física
  - [x] RN08 — inativar só troca a flag; filhas/endereço/vínculos intactos
- [x] `Controllers/`: `ClientesController` — GET (lista + `ativo` + `busca`),
      GET/{id}, POST, PUT/{id}, PATCH/{id}/inativar, PATCH/{id}/reativar
      (todos com `/// <summary>` + `[ProducesResponseType]`). Rota base `/clientes`.
- [x] Registrar DI de `IDbConnectionFactory`/`IClienteRepository`/`IClienteService` no `Program.cs`
- [x] Teste manual curl end-to-end contra `fogo_erp` (PF/PJ 201, RN05 409,
      inativar→reusar CPF 201, filtros, busca, 404, `/doc`). Dados de teste limpos.
- [ ] Tela de listagem (cards Total/Ativos/Inativos/Exibindo + busca + filtro + exportar)
- [ ] Modal de cadastro/edição com abas (Dados gerais / Endereço), alternando campos PF x PJ
- [ ] Teste manual do fluxo completo contra as RNs de `comercial.md`
- [ ] Fechar: `ai/plan.md` (checkbox Estória 01), `ai/context.md`, `ai/changelog.md`;
      merge `--no-ff` em `main` + push
