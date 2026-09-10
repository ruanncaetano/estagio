Implemente a Estória de Usuário informada como argumento ($ARGUMENTS),
seguindo este processo:

1. Localize a estória em `docs/requisitos/` (buscar pelo número/nome nos
   arquivos: comercial.md, producao.md, operacao-eventos.md, financeiro.md,
   relatorios-dashboard.md).
2. Releia `docs/architecture.md` e `docs/conventions.md` antes de começar.
3. Confira `ai/plan.md` — a estória está na fase certa? As dependências dela
   já foram implementadas?
4. Quebre a estória em tarefas concretas usando TodoWrite, e também registre
   em `ai/tasks.md` se a implementação for atravessar mais de uma sessão.
5. Implemente, mapeando explicitamente cada RN relevante para uma validação
   no código (ver convenção de rastreabilidade em `docs/conventions.md`).
6. Ao concluir: atualize `ai/plan.md` (marcar checkbox da estória),
   `ai/context.md` (próximo passo) e `ai/changelog.md` (entrada datada).
7. Se alguma RN ficou pendente ou incerta, registre isso explicitamente em
   `ai/plan.md` — não deixe de fora silenciosamente.
