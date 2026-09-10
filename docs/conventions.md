# Convenções compartilhadas

> Convenções que atravessam banco de dados + backend + frontend. Detalhes
> específicos de cada camada ficam em `backend/CLAUDE.md` e
> `frontend/CLAUDE.md`.

## Nomenclatura
- **Banco de dados**: `snake_case`, tabelas no singular (`cliente`,
  `ficha_tecnica`, `orcamento_equipe`). Chave primária: `id_<entidade>`.
- **C# (backend)**: `PascalCase`, espelhando os nomes das entidades do
  `docs/diagrama-classes.md` (`Cliente`, `FichaTecnica`, `OrcamentoEquipe`).
- **TypeScript (frontend)**: `camelCase` para variáveis/funções, `PascalCase`
  para componentes e tipos.

## Padrão de "não excluir fisicamente"
Duas variações, conforme o tipo de entidade — não confundir uma com a outra:

| Tipo de entidade | Mecanismo | Exemplo de coluna/enum |
|---|---|---|
| Cadastral (Cliente, Fornecedor, Insumo, Ficha Técnica, Equipamento, Funcionário) | Inativação/reativação | `ativo: boolean` |
| Transacional com fluxo de estado (Orçamento, Contrato, Evento, Ficha de Produção, Conta a Pagar/Receber) | Situação com enum próprio, inclui estado "cancelado" quando aplicável | `situacao: string` (valores definidos por estória — ver `docs/requisitos/`) |

Nunca usar `DELETE` físico em nenhuma dessas tabelas.

## Rastreabilidade de regras de negócio
Toda validação de RN implementada deve ser identificável no código (via
comentário ou nome de método) com o número da RN e a estória de origem —
facilita auditar cobertura depois. Ex: `ValidarRN05_CpfCnpjUnicoEntreAtivos()`.

## Git / commits

Repositório remoto: `git@github.com:ruanncaetano/estagio.git`.
Decisão de 2026-09-09 — ver `docs/decisions.md`.

### Branches
- `main` — sempre em estado entregável (compila, sem trabalho pela metade).
- Uma branch por bloco de trabalho, criada a partir de `main`:
  - `feat/estoria-XX-<slug>` — implementação de estória (ex: `feat/estoria-01-clientes`).
  - `chore/<slug>` / `docs/<slug>` / `fix/<slug>` — o resto.

### Commits
- Padrão **Conventional Commits em pt-BR**: `<tipo>(<escopo>): <resumo no imperativo>`.
- Tipos: `feat`, `fix`, `docs`, `chore`, `refactor`, `test`.
- Referenciar a estória e a RN quando aplicável, no formato `(E01 RN05)`:
  - `feat(clientes): valida CPF único entre clientes ativos (E01 RN05)`
  - `docs(arquitetura): registra estrutura de camadas do backend`

### Fechamento de um bloco
Ao concluir um bloco (estória ou tarefa relevante) e validá-lo:
1. `git checkout main && git merge --no-ff <branch>` (mantém o merge visível no histórico).
2. `git push origin main`.
3. A branch pode ser mantida ou apagada — não há exigência.

Toda alteração relevante termina publicada em `main` no GitHub — não deixar
trabalho concluído só no repositório local.
