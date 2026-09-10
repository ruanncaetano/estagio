# Frontend — Convenções

Carregado automaticamente quando o agente trabalha dentro de `/frontend`.

## Stack
- React / Next.js / TypeScript

## Referência de UI
Os protótipos do ERS (seção 2.3, "Detalhamento com protótipo") mostram o
padrão visual esperado: sistema gerencial denso, com cards de contagem no
topo (Total/Ativos/Inativos/Exibindo), tabela com busca + filtros +
exportar, modal de cadastro com abas ("Dados gerais" / "Endereço" /
"Composição" etc.), botões de ação por linha (editar/excluir como ícones).
Seguir essa linguagem visual — **não usar como referência os projetos
pessoais anteriores do Ruan**, e sim as convenções de sistemas gerenciais
reais (ERPs/CRMs comerciais).

## Estrutura de pastas
> O projeto Next.js **ainda não foi criado** (esta pasta só tem este
> `CLAUDE.md`). A definir junto com a primeira tela implementada: App Router
> vs Pages Router, onde ficam componentes compartilhados vs. por módulo, etc.
> Registrar a decisão em `docs/decisions.md` se não for trivial.
>
> Nota: o backend segue Web API por **camadas** (não é ASP.NET MVC com
> Views) — o frontend React/Next.js continua sendo a camada de apresentação
> separada. Ver `docs/architecture.md`.

## Módulos de tela (espelham os módulos do backend/requisitos)
- Comercial: Clientes, Orçamentos, Contratos
- Produção: Insumos, Fichas Técnicas, Fornecedores, Ficha de Produção
- Operação: Equipamentos, Funcionários, Eventos
- Financeiro: Contas a Pagar, Contas a Receber
- Relatórios/Dashboard

## Regras de exibição vindas do ERS (não são só estética, são requisito)
- PDF do orçamento **nunca** expõe custo interno/margens — só o Preço Final
  (RN03 da estória "Geração de PDF do Orçamento").
- PDF do contrato deve exibir aviso de que não tem validade jurídica sem
  assinatura (RN03 da estória "Geração Visual do Contrato").
- Dashboard e todos os relatórios de saída são exclusivos do perfil
  Administrador — a tela/rota deve refletir essa restrição de acesso.
