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
- [ ] Configurar conexão MySQL: `appsettings.Local.json` (gitignored) ou
      `dotnet user-secrets` + leitura via `IConfiguration` em `Data/`
- [x] Migration/DDL das tabelas: `cliente`, `cliente_pf`, `cliente_pj` +
      `schema_migration` — `Data/Migrations/000..001` aplicados e testados
      (FK, charset utf8, join PF/PJ)
- [ ] `Models/Domain/`: `Cliente`, `ClientePf`, `ClientePj` (+ enum/tipo de `tipo_cliente`)
- [ ] `Models/Dtos/`: `CriarClienteRequest`, `AtualizarClienteRequest`, `ClienteResponse`
- [ ] `Repositories/`: `IClienteRepository` + `ClienteRepository` (sem DELETE físico — RN06)
- [ ] `Services/`: `IClienteService` + `ClienteService` com as validações de RN:
  - [ ] RN01 — acesso só Administrador e Vendedor/Comercial
  - [ ] RN02 — aceitar PF e PJ
  - [ ] RN03 / RN04 — CPF (PF) e CNPJ (PJ) opcionais; quando informados, válidos
  - [ ] RN05 — bloquear dois clientes **ativos** com mesmo CPF/CNPJ (mensagem clara)
  - [ ] RN06 / RN07 — inativação (`ativo`), nunca exclusão física
  - [ ] RN08 — inativar não quebra vínculo/histórico
- [ ] `Controllers/`: `ClientesController` — GET (lista + filtro ativo/inativo),
      GET/{id}, POST, PUT/{id}, PATCH/{id}/inativar, PATCH/{id}/reativar
- [ ] Registrar DI de `IClienteService`/`IClienteRepository` no `Program.cs`
- [ ] Tela de listagem (cards Total/Ativos/Inativos/Exibindo + busca + filtro + exportar)
- [ ] Modal de cadastro/edição com abas (Dados gerais / Endereço), alternando campos PF x PJ
- [ ] Teste manual do fluxo completo contra as RNs de `comercial.md`
- [ ] Fechar: `ai/plan.md` (checkbox Estória 01), `ai/context.md`, `ai/changelog.md`;
      merge `--no-ff` em `main` + push
