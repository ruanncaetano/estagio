# Fogo de Chão Buffet Experience — ERP

Contexto que TODA sessão do Claude Code deve carregar antes de tocar em código.

## O que é este projeto

Sistema de gestão (ERP) sob medida para o Fogo de Chão Buffet Experience,
cobrindo clientes, orçamentos, produção, contratos, eventos e financeiro.
Desenvolvido como projeto de estágio supervisionado (FIPP/Unoeste), com
potencial de evoluir para um produto SaaS.

## Onde está cada coisa

- **`docs/`** — verdade estável do produto (existiria mesmo sem agente
  nenhum): requisitos do ERS por módulo, arquitetura, convenções, modelo de
  dados, glossário.
  - `docs/requisitos/` — leia o `.md` do módulo correspondente **antes** de
    implementar qualquer estória. Não invente regra de negócio que não
    esteja lá — se faltar algo, pergunte.
  - `docs/architecture.md` — decisões de arquitetura vigentes (leia antes de
    mexer em Contrato/Evento/Ficha de Produção/Financeiro — o fluxo
    automático entre eles é a parte mais fácil de quebrar sem querer).
  - `docs/conventions.md` — nomenclatura e padrões compartilhados entre
    banco/backend/frontend.
  - `docs/decisions.md` — log de decisões tomadas que não estavam no ERS.
  - `docs/modelo-dados.md` / `docs/diagrama-classes.md` — schema de
    referência em Mermaid.
- **`ai/`** — estado de trabalho do agente (muda o tempo todo):
  - `ai/plan.md` — roadmap e status das 25 estórias.
  - `ai/tasks.md` — backlog granular da estória em andamento (sobrevive
    entre sessões; complementa o TodoWrite, que só dura a sessão atual).
  - `ai/context.md` — "onde parei" — atualizar sempre que pausar no meio de algo.
  - `ai/changelog.md` — log datado do que foi de fato entregue.
- **`backend/CLAUDE.md`** e **`frontend/CLAUDE.md`** — convenções
  específicas de cada camada, carregadas automaticamente quando o agente
  trabalha ali dentro.

## Stack
- Backend: C# / ASP.NET Web API
- Frontend: React / Next.js (TypeScript)
- Banco de dados: MySQL
- Monorepo: `/backend` e `/frontend` na raiz

## Regras que atravessam o sistema inteiro (resumo — detalhe em `docs/architecture.md`)

1. Nunca excluir fisicamente — inativação (cadastros) ou cancelamento
   (transacional), nunca `DELETE`.
2. CPF/CNPJ opcionais, mas únicos quando informados (Cliente, Fornecedor, Funcionário).
3. Histórico é sagrado — inativar/cancelar nunca quebra vínculo já usado.
4. **Evento é o hub operacional**: Contrato assinado → Evento → Ficha de
   Produção + Contas a Receber + Contas a Pagar, tudo automático, sem ação
   manual do usuário depois da assinatura.
5. Snapshot no momento certo: Orçamento→Contrato e Contrato→Evento copiam
   dados fixos; edição posterior na origem gera Adendo, não propaga.

## Fluxo de trabalho esperado

1. Ler `docs/requisitos/<modulo>.md` antes de começar a estória.
2. Quebrar em tarefas no TodoWrite (sessão atual) e registrar no
   `ai/tasks.md` se for algo que vai atravessar mais de uma sessão.
3. Ao terminar um bloco relevante: atualizar `ai/plan.md` (status da
   estória), `ai/context.md` (onde parou) e, se for o caso, `ai/changelog.md`;
   depois commitar na branch do bloco e publicar em `main` no GitHub (ver
   "Fluxo Git / GitHub" abaixo).
4. RN não implementada ou incerta: registrar como pendência em `ai/plan.md`,
   nunca deixar silenciosamente de fora.
5. Decisão de arquitetura não coberta pelo ERS: perguntar antes, e registrar
   em `docs/decisions.md` depois de decidida.

### Fluxo Git / GitHub

Repositório: `git@github.com:ruanncaetano/estagio.git`. Detalhe e exemplos em
`docs/conventions.md` ("Git / commits").

- `main` sempre entregável; **uma branch por bloco** de trabalho
  (`feat/estoria-XX-<slug>`, `chore/<slug>`, `docs/<slug>`), criada de `main`.
- Commits em Conventional Commits pt-BR, referenciando estória/RN quando
  houver: `feat(clientes): valida CPF único entre ativos (E01 RN05)`.
- Ao fechar o bloco: `git checkout main && git merge --no-ff <branch>` e
  `git push origin main`. **Nenhuma alteração relevante fica só no local** —
  toda entrega termina publicada em `main` no GitHub.
- Backend restruturado em Web API por camadas (Controllers / Models /
  Services / Repositories / Data) — ver `backend/CLAUDE.md`.

## O que NÃO fazer sem perguntar

- Não mudar o stack.
- Não trocar inativação/cancelamento por exclusão física.
- Não implementar a integração de assinatura eletrônica sem confirmar a
  plataforma (ainda em definição — ver `docs/requisitos/comercial.md`, RN09/RN10).
- Não presumir a base de cálculo do "lucro apurado" (pendência registrada em
  `docs/requisitos/relatorios-dashboard.md`, RN04 da Estória 23).

## Modo de trabalho com C#
Ruan está usando este projeto também pra praticar/aprender C#. Prefira
guiar com perguntas antes de escrever código pronto em decisões de
modelagem — ver `backend/CLAUDE.md` para o detalhe.
