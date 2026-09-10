# Módulo Financeiro

Estórias: 08 (Contas a Pagar), 09 (Contas a Receber), 15 (Geração automática
de Conta a Receber), 16 (Geração automática de Conta a Pagar por funcionário).

---

## Estória 08 — Gerenciar Contas a Pagar

**Estória**: Como administrador, quero cadastrar, consultar, editar,
cancelar e registrar pagamentos de contas a pagar, vinculadas a fornecedores
ou funcionários, para manter o controle financeiro das despesas da empresa.

### Campos
- Data de lançamento, Descrição, Valor, Data de vencimento,
  Categoria/tipo de despesa, Vínculo obrigatório com Fornecedor **ou**
  Funcionário, Número de parcelas.
- Pagamento: data do pagamento, forma de pagamento, valor pago (permite
  múltiplos pagamentos parciais).

### Comportamento
- Cada parcela é um **registro independente** de conta a pagar, com
  vencimento e pagamento próprios.
- Situação calculada automaticamente com base nos pagamentos: **Em aberto,
  Parcialmente paga, Paga, Vencida**.
- Geração automática a partir da vinculação de funcionários a
  eventos/contratos (ver Estória 16).

### Regras de negócio
- RN01 — Apenas Administrador cadastra/gerencia contas a pagar.
- RN02 — Toda conta a pagar vinculada a Fornecedor OU Funcionário.
- RN03 — Permitir contas duplicadas (mesma descrição/valor/vencimento).
- RN04 — Conta parcelada gera um registro individual por parcela.
- RN05 — Múltiplos pagamentos parciais até quitação total.
- RN06 — Situação: Em aberto, Parcialmente paga, Paga ou Vencida
  (automática, com base nos pagamentos).
- RN07 — Não excluir fisicamente — apenas cancelar.
- RN08 — Histórico de pagamentos de conta cancelada é preservado.

---

## Estória 09 — Gerenciar Contas a Receber

**Estória**: Como administrador, quero cadastrar, consultar, editar,
cancelar e registrar recebimentos de contas a receber, vinculadas ou não a
um cliente/contrato, para manter o controle financeiro das receitas da
empresa.

### Campos
- Data de lançamento, Descrição, Valor, Data de vencimento, Categoria/tipo,
  Cliente vinculado, Referência a contrato/evento (**opcional** — permite
  cobrança avulsa), Número de parcelas.
- Sinal de evento: registrado como conta a receber própria, valor definido
  manualmente (sem percentual padrão sugerido).
- Recebimento: data, forma, valor (múltiplos recebimentos parciais).

### Regras de negócio
- RN01 — Apenas Administrador cadastra/gerencia contas a receber.
- RN02 — Conta pode ou não estar vinculada a contrato/evento.
- RN03 — Não permitir duas contas **em aberto** para o mesmo contrato/evento.
- RN04 — Conta parcelada gera registro individual por parcela.
- RN05 — Sinal registrado como conta a receber, valor manual, sem % padrão.
- RN06 — Múltiplos recebimentos parciais até quitação total.
- RN07 — Situação: Em aberto, Parcialmente recebida, Recebida ou Vencida
  (automática).
- RN08 — Não excluir fisicamente — apenas cancelar.
- RN09 — Histórico de recebimentos de conta cancelada é preservado.

---

## Estória 15 — Geração Automática de Conta a Receber a partir do Contrato

**Estória**: Como sistema, quero gerar automaticamente as contas a receber
(sinal + parcelas do restante) no momento da criação do Evento, para
assegurar o controle financeiro do evento sem depender de lançamento manual.

### Comportamento e cálculo
- Disparado junto com a criação do Evento (que por sua vez é disparada pela
  assinatura do contrato).
- **Sinal**: valor = `valorAdiantamento` do contrato; vencimento = data de
  assinatura do contrato; marcada como sinal (`ehSinal = true`).
- **Restante**: `valorTotal - valorAdiantamento`, dividido em partes iguais
  conforme `numeroParcelasRestante` do contrato.
- Vencimento de cada parcela do restante = múltiplos de 30 dias a partir da
  assinatura (1ª = +30, 2ª = +60, ...).
- Todas as contas vinculadas ao cliente e ao contrato/evento de origem.
- Seguem o mesmo CRUD da Estória 09, editáveis após a geração.

### Regras de negócio
- RN01 — Geração automática, junto com a criação do Evento, sem ação manual.
- RN02 — Contrato deve ter campo `numeroParcelasRestante`.
- RN03 — Sinal sempre gerado como conta própria = `valorAdiantamento`.
- RN04 — Restante dividido em partes iguais entre as parcelas definidas.
- RN05 — Vencimento do sinal = data de assinatura do contrato.
- RN06 — Vencimento de cada parcela do restante = intervalos de 30 dias a
  partir da assinatura.
- RN07 — Contas geradas seguem as mesmas regras de edição/cancelamento/
  recebimento da Estória 09.

---

## Estória 16 — Geração Automática de Conta a Pagar por Funcionário Vinculado

**Estória**: Como sistema, quero gerar automaticamente uma conta a pagar
para cada funcionário vinculado à equipe da Ficha de Produção, no momento
da criação do Evento, para assegurar o controle financeiro da mão de obra
sem depender de lançamento manual.

### Comportamento e cálculo
- Disparado junto com a criação do Evento (e consequente Ficha de Produção).
- Uma conta a pagar por funcionário presente na **equipe da Ficha de
  Produção** (não a equipe original do orçamento — atenção a essa diferença
  de origem).
- Valor = `valorDiaria × quantidade` do item de equipe correspondente.
- Vencimento = data do evento + 30 dias.
- Cada funcionário gera **uma única** conta, sem parcelamento.
- Seguem o mesmo CRUD da Estória 08, editáveis após a geração.

### Regras de negócio
- RN01 — Geração automática junto com Evento + Ficha de Produção, sem ação manual.
- RN02 — Origem dos funcionários = equipe da **Ficha de Produção do Evento**,
  não a equipe original do Orçamento.
- RN03 — Valor da conta = subtotal calculado (`valorDiaria × quantidade`).
- RN04 — Vencimento = data do evento + 30 dias.
- RN05 — Um funcionário → uma conta, sem parcelamento.
- RN06 — Contas seguem as mesmas regras de edição/cancelamento/pagamento da
  Estória 08.
