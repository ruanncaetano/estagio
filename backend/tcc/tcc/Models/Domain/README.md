# Models/Domain — o "M" do MVC

Entidades de domínio em `PascalCase`, espelhando `docs/diagrama-classes.md`
e `docs/modelo-dados.md` (ex: `Cliente`, `FichaTecnica`, `OrcamentoEquipe`).

- Uma classe por entidade. Sem lógica de acesso a dados, sem atributos de
  serialização de API (isso é DTO).
- Coluna de "não excluir fisicamente": `Ativo` (bool) para cadastros,
  `Situacao` (enum/string própria da estória) para transacionais —
  ver `docs/conventions.md`.

Um `.gitkeep` mantém a pasta versionada enquanto estiver vazia.
