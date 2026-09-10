---
name: frontend-conectar-api
description: >-
  Liga uma tela do Fogo de Chão ERP a um endpoint do backend: gera os tipos
  TypeScript da entidade e dos DTOs, o client HTTP e os hooks de dados
  (listar/obter/criar/atualizar/inativar), respeitando perfil de acesso e as
  regras de exibição de RN. Use depois de montar a tela (skill
  frontend-scaffold-tela) ou ao trocar dados mock por chamadas reais.
---

# Conectar tela à API

Objetivo: uma camada de dados tipada e consistente entre telas, alinhada ao
contrato real do backend.

## 1. Ler o contrato do backend

- Localize o controller/DTOs da entidade em `/backend` e/ou a estória em
  `docs/requisitos/`. Anote: rotas, shape do request e do response, valores
  de `situacao`/`ativo`, e campos que o response **não** traz (custo
  interno, margem — não invente que existem).
- Se o endpoint ainda não existe, pare: alinhe o contrato antes de codar o
  client (ou combine um mock explícito e registre como pendência em
  `ai/plan.md`).

## 2. Tipos TypeScript

- Um tipo por entidade (`PascalCase`) espelhando o `Response` do backend.
- Tipos separados para payload de escrita: `CriarXInput`, `AtualizarXInput`.
- `situacao` como union de literais com os valores exatos da estória, não
  `string` solto.
- Nada de campo proibido pelo ERS no tipo de leitura.

## 3. Client HTTP

- Espelhe o padrão já usado no projeto; se for o primeiro, proponha (fetch
  wrapper próprio vs. axios) e pergunte. Registre a decisão.
- Um módulo por entidade: `listar(params)`, `obter(id)`, `criar(input)`,
  `atualizar(id, input)`, `inativar(id)` / `cancelar(id)`. **Nunca** um
  `excluir` com DELETE — inativar/cancelar chama o endpoint de `UPDATE` de
  situação.
- Trate erro de forma uniforme (mesma forma de `{ mensagem, campos? }` para
  a tela consumir).

## 4. Hooks de dados

- Um hook de listagem que recebe busca + filtros + paginação e devolve
  `{ dados, carregando, erro, recarregar }`, alimentando também os cards de
  contagem (Total / Ativos / Inativos / Exibindo).
- Hooks de mutação (`criar`/`atualizar`/`inativar`) que expõem estado de
  envio e revalidam a listagem no sucesso.
- Siga a lib de data-fetching já adotada no projeto (React Query, SWR ou
  hooks manuais); não introduza uma nova sem perguntar.

## 5. Perfil de acesso

- Se o RN01 restringe, a camada de dados não deve nem disparar a chamada
  para perfil sem permissão — o guard fica na rota/tela, mas o hook não
  assume acesso.

## 6. Fechar

- Troque todo dado mock pela chamada real; remova TODOs resolvidos.
- `npm run lint` / `npm run build` — relate o resultado real.
- Atualize `ai/context.md` e `ai/changelog.md`.
