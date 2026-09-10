---
name: backend-scaffold-modulo
description: >-
  Estrutura um módulo/endpoint novo de backend do Fogo de Chão ERP (C# /
  ASP.NET + MySQL) seguindo as convenções do projeto — entidade de domínio,
  DTOs, service, controller, repository e migration — já com os comentários
  de rastreabilidade de RN no lugar. Use ao começar a implementação de
  qualquer estória de cadastro ou de fluxo transacional no backend.
---

# Scaffold de módulo backend

Objetivo: produzir a espinha dorsal de um módulo do backend consistente com
o resto do sistema, sem inventar regra de negócio e sem esquecer as regras
que atravessam o ERP.

## 1. Levantar a verdade da estória

- Abra `docs/requisitos/<modulo>.md` e localize a estória (número/nome).
- Extraia: campos da entidade, RNs numeradas, quem acessa (RN01), mecanismo
  de "não excluir" (é cadastral com `ativo` ou transacional com `situacao`?),
  e valores exatos do enum de `situacao` se houver.
- Cruze os campos com `docs/modelo-dados.md` e `docs/diagrama-classes.md`.
  Divergência entre ERS e modelo → **pergunte antes**, não escolha sozinho.

## 2. Confirmar o padrão de camadas

Se esta é a **primeira** estória de backend, a arquitetura de camadas ainda
não existe. NÃO gere o scaffold direto: proponha 2 opções
(Controllers/Services/Repositories clássico vs. Vertical Slice; DTO manual
vs. AutoMapper) com prós e contras curtos e pergunte ao Ruan. Depois de
decidido, registre em `backend/CLAUDE.md` e `docs/decisions.md`, e só então
siga.

Se já existe um módulo implementado, **espelhe a estrutura dele** — mesmas
pastas, mesmos nomes de sufixo, mesmo estilo de mapeamento.

## 3. Gerar os artefatos

Para a entidade `X`:

- **Domínio** `X` (`PascalCase`): propriedades espelhando o diagrama de
  classes. Coluna de não-exclusão conforme o tipo:
  - cadastral → `bool Ativo`
  - transacional → `string Situacao` (ou enum) com os valores da estória
- **DTOs**: `CriarXRequest`, `AtualizarXRequest`, `XResponse`. O `Response`
  nunca vaza campo que o perfil/estória proíbe (ex: custo interno, margem).
- **Repository**: métodos de leitura filtram por `Ativo`/`Situacao` conforme
  o caso; escrita nunca faz `DELETE` — inativação/cancelamento é `UPDATE`.
- **Service**: uma função de validação por RN relevante, nomeada pela RN
  (ver passo 4). Orquestra o repository.
- **Controller**: rotas REST; atributo de autorização refletindo o RN01.
- **Migration**: tabela `snake_case` singular, PK `id_<entidade>`,
  unicidade de CPF/CNPJ entre ativos quando a entidade tiver documento.

## 4. Amarrar a rastreabilidade de RN

Para cada RN da estória que vira validação, crie um método identificável:

```csharp
// RN05 (Estória 01) — não permitir dois clientes ativos com mesmo CPF/CNPJ
private void ValidarRN05_CpfCnpjUnicoEntreAtivos(string documento) { ... }
```

RN que não deu para implementar agora → linha explícita em `ai/plan.md` na
seção de pendências da estória. Nunca omita em silêncio.

## 5. Regras que não podem passar batido

- Snapshot: se o módulo copia dados de outra entidade (Contrato←Orçamento,
  Evento←Contrato), a cópia é fixa; alteração na origem gera Adendo, não
  propaga.
- Cadeia automática: se o módulo é Evento/Ficha de Produção/Financeiro, o
  disparo vem da mudança de `situacao` do Contrato para "Assinado", sem ação
  manual. Confirme em `docs/architecture.md` e nos requisitos antes.
- Histórico: inativar/cancelar não pode quebrar vínculo já usado.

## 6. Fechar

- `dotnet build` e `dotnet test` — relate o resultado real.
- Atualize `ai/plan.md`, `ai/context.md`, `ai/changelog.md`.
