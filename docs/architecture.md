# Arquitetura — estado atual

> Este arquivo descreve **como o sistema é hoje** (decisão vigente). O
> histórico de *como chegamos aqui* e alternativas descartadas fica em
> `docs/decisions.md`.

## Stack
- Backend: C# / ASP.NET Web API
- Frontend: React / Next.js (TypeScript)
- Banco de dados: MySQL
- Hospedagem: aplicação web hospedada em servidor, acesso via navegador
  (sem instalação local pelo usuário final)

## Estrutura do monorepo
```
fogo-erp/
├── backend/     # API ASP.NET
├── frontend/    # Next.js
├── docs/        # verdade estável do projeto (requisitos, modelo, convenções)
├── ai/          # estado de trabalho do agente (plan, tasks, context, changelog)
└── .claude/     # config do Claude Code
```

## Estrutura de camadas do backend

Web API em camadas (decisão de 2026-09-09 — ver `docs/decisions.md`). Projeto
único `backend/tcc/tcc/`:

```
Controllers/     # entrada/saída HTTP — sem regra de negócio
Models/Domain/   # entidades de domínio (espelham docs/diagrama-classes.md)
Models/Dtos/     # contratos de request/response da API
Services/        # regra de negócio — todas as validações de RN moram aqui
Repositories/    # único ponto de acesso ao MySQL — nunca DELETE físico
Data/            # conexão/contexto de persistência
```

Fluxo de request: `Controller → Service → Repository → MySQL`. Interface +
implementação por módulo, ligadas por injeção de dependência no `Program.cs`.
Detalhe em `backend/CLAUDE.md`.

Persistência: **Dapper** sobre MySQL 5.5 (EF Core moderno não suporta 5.5).
Schema versionado em scripts SQL numerados (`Data/Migrations/`).

Transversais da API (decisão de 2026-09-09):
- **OpenAPI obrigatório** — `Microsoft.AspNetCore.OpenApi` gera o documento
  (`/openapi/v1.json`), Scalar serve a UI de teste em `/doc`. Todo endpoint
  documentado.
- **Serilog** — console + arquivo rotativo; `ExceptionHandlingMiddleware`
  centraliza o log de erros e responde `500` padronizado.

## O hub operacional: Evento

Decisão central do sistema — vale entender antes de mexer em qualquer
módulo que toque produção ou financeiro:

```
Orçamento (Aprovado)
      │  [ação explícita do usuário: "Gerar Contrato"]
      ▼
Contrato (Aguardando Assinatura → Assinado)
      │  [mudança de situação para "Assinado" dispara tudo abaixo, automático]
      ▼
Evento (Agendado)
      ├──▶ Ficha de Produção (a partir daqui, vinculada ao Evento — não mais ao Orçamento)
      ├──▶ Contas a Receber (sinal + parcelas do restante)
      └──▶ Contas a Pagar (uma por funcionário da equipe da Ficha de Produção)
```

Nada disso é acionado manualmente pelo usuário a partir do momento em que o
contrato é assinado — é uma cadeia automática. Ver
`docs/requisitos/operacao-eventos.md` e `docs/requisitos/financeiro.md`
para o detalhe de cada disparo.

## Padrão de dado: snapshot vs. referência viva

- **Orçamento → Contrato**: snapshot (cópia fixa) de cliente, valor total,
  data/hora/local do evento, quantidade de convidados. Edições no orçamento
  original depois disso **não** propagam — geram um Adendo.
- **Contrato → Evento**: também snapshot dos mesmos dados.
- **Ficha de Produção**: gerada a partir do Evento com dados herdados
  (cardápio, equipe, equipamentos), mas **editável manualmente** depois —
  não é um snapshot congelado, é um ponto de partida.

## Padrão de dado: nunca excluir fisicamente

Toda entidade cadastral (Cliente, Fornecedor, Insumo, Ficha Técnica,
Equipamento, Funcionário) usa inativação/reativação. Entidades
transacionais com estado de negócio (Contrato, Conta a Pagar/Receber) usam
**cancelamento**, que é semanticamente diferente de inativação mas também
nunca é exclusão física. Ver `docs/conventions.md` para como isso se traduz
em schema.

## Perfis de acesso
- **Administrador**: acesso completo, único perfil com acesso a relatórios
  e dashboard.
- **Vendedor/Comercial**: clientes, orçamentos, contratos, fornecedores,
  equipamentos.
- **Equipe de Cozinha**: consulta e gestão da Ficha de Produção.

Cada estória em `docs/requisitos/` especifica no RN01 quem tem acesso —
essa é a fonte de verdade por funcionalidade; a lista acima é só o resumo.
