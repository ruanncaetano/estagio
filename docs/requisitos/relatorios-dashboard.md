# Módulo Relatórios / Dashboard

Estórias: 17 (Dashboard), 18 (Histórico de preços), 19 (PDF do Contrato),
20 (PDF do Orçamento), 21 (PDF da Ficha de Produção), 22 (Necessidade de
compras), 23 (Distribuição de lucros), 24 (Contas a Receber por período),
25 (Contas a Pagar por período).

Todas as estórias deste módulo têm em comum: **acesso restrito ao perfil
Administrador**, exceto a 21 (Ficha de Produção), acessível também à equipe
de Cozinha.

---

## Estória 17 — Dashboard com Indicadores Principais do Negócio

### Comportamento
- Filtrável por período: mês, trimestre, ano ou intervalo customizado.
- Cards com: Total de eventos no período, Faturamento (soma de contas a
  receber **recebidas**), Despesas (soma de contas a pagar **pagas**),
  Saldo (Faturamento − Despesas), Ticket médio (Faturamento ÷ qtd. de
  eventos/contratos no período), Total de contas a receber em aberto, Total
  de contas a pagar em aberto, Qtd. de orçamentos em aberto vs. aprovados.
- Lista de próximos eventos agendados.
- Gráficos de evolução de faturamento e despesas ao longo do período.

### Regras de negócio
- RN01 — Apenas Administrador acessa.
- RN02 — Indicadores recalculados dinamicamente conforme filtro de período.
- RN03 — "Faturamento" = só valores efetivamente **recebidos** (não o total
  em aberto).
- RN04 — "Despesas" = só valores efetivamente **pagos**.
- RN05 — Ticket Médio = base nos contratos assinados dentro do período.
- RN06 — Indicadores "em aberto" de contas a pagar/receber **não** são
  filtrados por período — refletem sempre a situação atual.

---

## Estória 18 — Relatório de Histórico de Preços de Insumos por Fornecedor

### Comportamento
- Lista o histórico completo de preços (insumo, fornecedor, preço, data),
  sem o limite de "3 últimos" que a tela de cadastro do insumo usa.
- Filtros combináveis: insumo, fornecedor, período.
- Exportação em PDF.

### Regras de negócio
- RN01 — Apenas Administrador acessa.
- RN02 — Exibe todo o histórico, sem limite.
- RN03 — Filtros combináveis simultaneamente.
- RN04 — Exportável em PDF.

---

## Estória 19 — Geração Visual do Contrato (PDF)

### Comportamento
- Botão de download disponível em qualquer situação do contrato.
- Conteúdo: número/data de emissão, dados do cliente, dados do evento,
  cardápio/itens contratados, valor total/adiantamento/forma de pagamento,
  condições/cláusulas.
- Deve exibir aviso de que **não tem validade jurídica sem assinatura**.
- Não substitui o fluxo de assinatura eletrônica (integração futura).

### Regras de negócio
- RN01 — Administrador e Vendedor/Comercial podem gerar.
- RN02 — Independente da situação do contrato.
- RN03 — Aviso obrigatório de validade jurídica condicionada à assinatura.
- RN04 — Independente do processo de assinatura eletrônica.

---

## Estória 20 — Geração de PDF do Orçamento

### Comportamento
- Voltado para apresentação ao cliente — botão disponível em qualquer
  situação do orçamento.
- Conteúdo: dados do cliente, data/hora/local, convidados, cardápio, equipe/
  custos operacionais e diversos (descrição dos itens, **sem valores
  internos**), Preço Final.
- **Nunca** expor Custo Total, Margem de Segurança, Margem de Lucro ou
  Custo Ajustado — só o Preço Final.

### Regras de negócio
- RN01 — Administrador e Vendedor/Comercial podem gerar.
- RN02 — Independente da situação do orçamento.
- RN03 — Não expor composição de custo/margens internas.

---

## Estória 21 — Impressão/Exportação da Ficha de Produção

### Comportamento
- Disponível em qualquer situação da ficha.
- Conteúdo: dados do evento, cardápio com instruções/tempo/antecedência,
  insumos necessários já escalados, equipe alocada, equipamentos reservados.
- Deve refletir ajustes manuais feitos após a geração automática.

### Regras de negócio
- RN01 — Administrador e equipe de Cozinha podem gerar.
- RN02 — Independente da situação da ficha.
- RN03 — Reflete dados atuais (incluindo ajustes manuais).

---

## Estória 22 — Relatório de Necessidade de Compras do Evento

### Comportamento
- Dentro do painel de gestão do Evento: lista insumos necessários (já
  escalados), consolidados a partir da Ficha de Produção.
- Sem dedução de estoque (não há controle de estoque de insumos).
- Filtro "Fornecedores indicados": sugere, por insumo, o fornecedor com
  **menor preço médio** cadastrado.
- Exportável em PDF.

### Regras de negócio
- RN01 — Apenas Administrador acessa.
- RN02 — Sempre no contexto de um evento específico.
- RN03 — Quantidade = necessidade total, sem dedução de estoque.
- RN04 — Filtro "Fornecedores indicados" usa o preço médio (ver Estória 03).
- RN05 — Exportável em PDF.

---

## Estória 23 — Relatório de Distribuição de Lucros por Sócio

### Comportamento
- Cadastro de Sócios em área de Parâmetros do Sistema: nome + percentual de
  participação (editável).
- Relatório filtrável por período, calculando o valor de cada sócio =
  `percentual × lucro apurado no período`.
- Exportável em PDF.

### Regras de negócio
- RN01 — Apenas Administrador acessa e gerencia o cadastro de sócios.
- RN02 — Cada sócio tem percentual individual.
- RN03 — Valor = percentual × lucro apurado no período.
- RN04 — **[Pendência]** Base de cálculo do "lucro apurado" ainda não está
  fechada. Provisoriamente, usar o Saldo do Dashboard (Faturamento recebido
  − Despesas pagas) como referência, até definição final.
- RN05 — Soma dos percentuais dos sócios **não** é validada automaticamente
  nesta versão.

---

## Estória 24 — Relatório de Contas a Receber por Período

### Comportamento
- Lista: cliente, descrição, valor, vencimento, situação, data de
  recebimento (quando houver).
- Filtros combináveis: período (vencimento), cliente, situação.
- Resumo agrupado por mês, somando valores por situação.
- Exportável em PDF.

### Regras de negócio
- RN01 — Apenas Administrador acessa.
- RN02 — Filtros combináveis simultaneamente.
- RN03 — Resumo mensal soma valores por situação dentro do filtro.
- RN04 — Exportável em PDF.

---

## Estória 25 — Relatório de Contas a Pagar por Período

### Comportamento
- Lista: fornecedor/funcionário vinculado, descrição, valor, vencimento,
  situação, data de pagamento (quando houver).
- Filtros combináveis: período (vencimento), fornecedor, funcionário, situação.
- Resumo agrupado por mês, somando valores por situação.
- Exportável em PDF.

### Regras de negócio
- RN01 — Apenas Administrador acessa.
- RN02 — Filtros combináveis simultaneamente.
- RN03 — Resumo mensal soma valores por situação dentro do filtro.
- RN04 — Exportável em PDF.
