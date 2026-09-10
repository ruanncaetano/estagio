---
name: frontend-scaffold-tela
description: >-
  Monta a tela gerencial padrão do Fogo de Chão ERP em React / Next.js /
  TypeScript — cards de contagem no topo, tabela com busca/filtros/exportar,
  modal de cadastro com abas e ações de linha por ícone — já com os estados
  de carregando/vazio/erro e a restrição de perfil de acesso. Use ao começar
  a implementação de qualquer tela de listagem/cadastro do sistema.
---

# Scaffold de tela gerencial

Objetivo: gerar uma tela consistente com a linguagem visual do ERS (ERP/CRM
comercial denso), sem reinventar layout a cada módulo.

## 1. Levantar a verdade da estória

- Abra `docs/requisitos/<modulo>.md`, localize a estória e leia a seção 2.3
  ("Detalhamento com protótipo") — ela define abas do modal, colunas da
  tabela e filtros.
- Extraia: campos e agrupamento em abas, colunas da tabela, filtros, quem
  acessa (RN01), e as RNs de **exibição** (o que esconder, o que avisar).
- Confirme com `ai/plan.md` que o endpoint consumido já existe no backend.

## 2. Confirmar a estrutura de pastas

Se esta é a **primeira** tela, a estrutura de `/frontend` ainda não existe.
NÃO gere direto: proponha App Router vs. Pages Router e onde ficam
componentes compartilhados vs. por módulo, pergunte ao Ruan, registre em
`frontend/CLAUDE.md` (e em `docs/decisions.md` se não-trivial), e só então
siga. Se já existe uma tela, **espelhe a estrutura dela**.

## 3. Blocos da tela

1. **Cabeçalho** — título do módulo + botão primário "Novo <entidade>".
2. **Cards de contagem** — Total / Ativos / Inativos / Exibindo. "Exibindo"
   reflete o resultado após busca e filtros.
3. **Barra de ferramentas** — campo de busca, filtros (pelo menos
   ativo/inativo, mais os que a estória pedir), botão Exportar.
4. **Tabela** — colunas da estória; última coluna com ações por linha como
   **ícones**: Editar e Inativar/Reativar (cadastral) ou Cancelar
   (transacional). Nunca "Excluir". Paginação se a listagem crescer.
5. **Modal de cadastro/edição** — abas conforme a estória ("Dados gerais" /
   "Endereço" / "Composição" …). Validação inline de campo. Um mesmo modal
   serve criar e editar.

## 4. Estados obrigatórios

- **Carregando**: skeleton na tabela e nos cards.
- **Vazio**: mensagem + atalho para "Novo <entidade>".
- **Erro de carga**: mensagem + botão "Tentar de novo".
- **Salvando**: botão do modal desabilitado + spinner; erro de submit
  mostrado no modal sem fechá-lo.
- Item inativo **continua aparecendo** quando o filtro inclui Inativos —
  nunca some da tela.

## 5. Perfil de acesso e regras de exibição

- Se o RN01 restringe o acesso, a rota/tela bloqueia perfis sem permissão
  (redireciona ou mostra "sem acesso"). Dashboard e relatórios são só
  Administrador.
- Regras de exibição de RN não são estética — trate cada uma (ex.: não
  mostrar custo interno/margem; avisos obrigatórios em PDFs).

## 6. Fechar

- `npm run lint` e `npm run build` — relate o resultado real.
- Deixe a ligação com a API pronta ou marcada com TODO claro (ver skill
  `frontend-conectar-api`).
- Atualize `ai/plan.md`, `ai/context.md`, `ai/changelog.md`.
