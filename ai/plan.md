# Plano do Projeto — Fogo de Chão ERP

> Memória persistente entre sessões do Claude Code. TodoWrite cuida do
> detalhe tático de UMA sessão; `ai/tasks.md` cuida do backlog granular que
> atravessa várias sessões; aqui fica o estado macro — o que já foi feito,
> o que falta, pendências que não podem se perder. Ver também
> `ai/context.md` (onde parei agora) e `ai/changelog.md` (histórico do que
> foi entregue).

## Status geral

Fase atual: **estruturação concluída, pronto para a Estória 01**. Nenhuma
estória de negócio implementada ainda.

### Fase 0 — Estruturação (concluída em 2026-09-09)
- [x] Documentação do produto (`docs/`) e memória do agente (`ai/`).
- [x] Repositório git inicializado e publicado em
      `git@github.com:ruanncaetano/estagio.git`.
- [x] Fluxo Git/GitHub definido (branch por bloco + merge `--no-ff` + push) —
      `docs/conventions.md`.
- [x] Backend reestruturado em Web API por camadas (Controllers / Models /
      Services / Repositories / Data) — `backend/CLAUDE.md`,
      `docs/architecture.md`. Só esqueleto: `HealthController` + READMEs de
      camada, sem regra de negócio.

## Ordem sugerida de implementação

A ordem segue as dependências reais entre as estórias (não dá pra fazer
Contrato antes de Orçamento, nem Ficha de Produção antes de Ficha Técnica):

### Fase 1 — Cadastros básicos (sem dependências entre si)
- [~] Estória 01 — Gerenciar Clientes _(backend C# completo e testado via curl;
      falta o frontend e a RN01/auth — ver `ai/tasks.md` e pendência abaixo)_
- [ ] Estória 05 — Gerenciar Fornecedores
- [ ] Estória 06 — Gerenciar Equipamentos
- [ ] Estória 10 — Gerenciar Funcionários
- [ ] Estória 03 — Gerenciar Insumo

### Fase 2 — Produção (depende de Insumo/Fornecedor)
- [ ] Estória 04 — Gerenciar Ficha Técnica (depende de Insumo)

### Fase 3 — Comercial (depende de Cliente, Ficha Técnica, Funcionário)
- [ ] Estória 02 — Gerenciar Orçamentos
- [ ] Estória 11 — Cálculo automático do valor do orçamento (parte do fluxo do orçamento)
- [ ] Estória 07 — Gerenciar Contratos
- [ ] Estória 12 — Conversão de Orçamento Aprovado em Contrato

### Fase 4 — Operação (depende de Contrato assinado)
- [ ] Estória 13 — Conversão de Contrato Assinado em Evento
- [ ] Estória 14 — Geração e Gestão da Ficha de Produção do Evento

### Fase 5 — Financeiro automático (depende de Evento/Ficha de Produção)
- [ ] Estória 08 — Gerenciar Contas a Pagar
- [ ] Estória 09 — Gerenciar Contas a Receber
- [ ] Estória 15 — Geração automática de Conta a Receber a partir do Contrato
- [ ] Estória 16 — Geração automática de Conta a Pagar por funcionário vinculado

### Fase 6 — Saídas / relatórios (dependem de tudo acima já ter dados)
- [ ] Estória 17 — Dashboard com indicadores principais
- [ ] Estória 18 — Relatório de histórico de preços de insumos por fornecedor
- [ ] Estória 19 — Geração visual do contrato (PDF)
- [ ] Estória 20 — Geração de PDF do orçamento
- [ ] Estória 21 — Impressão/exportação da ficha de produção
- [ ] Estória 22 — Relatório de necessidade de compras do evento
- [ ] Estória 23 — Relatório de distribuição de lucros por sócio
- [ ] Estória 24 — Relatório de contas a receber por período
- [ ] Estória 25 — Relatório de contas a pagar por período

## Pendências técnicas em aberto (decisões do agente, não do ERS)

- ~~ORM do backend não decidido~~ → **Dapper + migrations SQL manuais**
  (MySQL 5.5 não suporta EF Core moderno). Decidido em 2026-09-09 —
  `docs/decisions.md`.
- **Runner de migrations** — hoje os scripts `.sql` são aplicados na mão via
  cliente `mysql`. Um runner que aplica os pendentes automaticamente pode
  entrar quando incomodar.
- **Rename do projeto `tcc` → algo como `FogoErp.Api`** — follow-up opcional,
  sem urgência.

## Pendências de RN não implementadas (rastreamento obrigatório)

- **E01 RN01 (Gerenciar Clientes)** — "Apenas Administrador e
  Vendedor/Comercial podem cadastrar clientes". **Não implementada**: o
  projeto ainda não tem autenticação/autorização. Marcador `// E01 RN01 —
  TODO: restringir a Administrador/Vendedor quando houver auth` no
  `ClienteService`. Aplicar quando a camada de auth existir (provável estória
  transversal futura); revisitar todas as RN01 das demais estórias na mesma
  ocasião.

## Pendências abertas registradas no próprio ERS

- **Plataforma de assinatura eletrônica** ainda não definida (ERS 1.2, RN09 de
  Gerenciar Contratos).
- **Regras de edição de contrato após assinatura** dependem da plataforma
  escolhida acima (RN10 de Gerenciar Contratos).
- **Base de cálculo do "lucro apurado"** para distribuição de lucros por sócio
  não está fechada — ERS assume provisoriamente o Saldo do Dashboard (RN04 da
  Estória 23).
- **Referência da Ficha de Produção** hoje aponta pro Orçamento; será migrada
  para apontar ao Evento quando essa entidade for implementada (RN06 da
  Estória 14) — atenção pra não implementar a versão antiga por engano.
