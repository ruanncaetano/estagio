# Módulo Comercial

Estórias: 01 (Clientes), 02 (Orçamentos), 11 (Cálculo automático do
orçamento — embutido na 02), 07 (Contratos), 12 (Conversão Orçamento→Contrato).

---

## Estória 01 — Gerenciar Clientes

**Estória**: Como administrador ou vendedor/comercial, quero cadastrar,
consultar, editar e inativar clientes pessoa física ou jurídica, para manter
uma base de clientes organizada e confiável para uso em orçamentos, contratos
e eventos.

### Campos
- **Pessoa Física**: Nome, CPF, WhatsApp, E-mail, Endereço.
- **Pessoa Jurídica**: Razão Social, Nome Fantasia, CNPJ, Responsável,
  WhatsApp, E-mail, Endereço.

### Comportamento
- CPF/CNPJ não são obrigatórios, mas quando informados devem ser únicos —
  validação de unicidade antes de concluir o cadastro, com mensagem clara se
  já existir.
- Consulta de clientes alimenta orçamentos, contratos e eventos.
- Nunca excluir fisicamente — usar inativação, preservando histórico e
  vínculos. Cliente inativo continua consultável para histórico, mas não
  deve ser usado em novos processos que exijam cliente ativo (salvo regra
  específica que permita).

### Regras de negócio
- RN01 — Apenas Administrador e Vendedor/Comercial podem cadastrar clientes.
- RN02 — Permitir Pessoa Física e Pessoa Jurídica.
- RN03 — CPF opcional para PF; quando informado, único.
- RN04 — CNPJ opcional para PJ; quando informado, único.
- RN05 — Não permitir dois clientes **ativos** com o mesmo CPF ou CNPJ.
- RN06 — Clientes não devem ser excluídos fisicamente.
- RN07 — Cliente deve ter situação ativo/inativo.
- RN08 — Histórico do cliente preservado mesmo após inativação.
- RN09 — Dados do cliente associáveis a orçamentos, contratos e eventos.

---

## Estória 02 — Gerenciar Orçamentos (inclui Estória 11 — cálculo automático)

**Estória**: Como vendedor/comercial ou administrador, quero criar, consultar
e editar orçamentos de eventos, com cálculo automático do valor com base no
cardápio, equipe, custos operacionais e margens, para agilizar a elaboração
de propostas e reduzir divergências entre o que é orçado e o que é entregue.

### Campos do orçamento
- Cliente vinculado, Data/hora do evento, Local do evento, Quantidade de
  convidados, Observações.
- Uma ou mais Fichas Técnicas (cardápio/pratos).
- Equipe/mão de obra: função, quantidade, valor da diária (subtotal por
  item calculado).
- Custos operacionais: transporte/combustível, gás, energia, logística,
  outros.
- Custos diversos: lista dinâmica (descrição + valor) — aluguel de
  equipamentos, taxas, imprevistos.
- Margem de Segurança (%) — padrão 10%, editável por orçamento.
- Margem de Lucro (%) — padrão 30%, editável por orçamento.
- Situação: Aberto, Enviado, Aprovado.

### Cálculo do resumo (em tempo real)
```
Custo Ingredientes  = soma(custo das fichas técnicas selecionadas) × qtd_convidados
Custo Total         = Ingredientes + Equipe + Custos Operacionais + Custos Diversos
Custo Ajustado      = Custo Total + Margem de Segurança (%)
Preço Final         = Custo Ajustado + Margem de Lucro (%)
Valor Por Pessoa    = Preço Final ÷ qtd_convidados
```

### Regras de negócio
- RN01 — Apenas Administrador e Vendedor/Comercial criam/gerenciam orçamentos.
- RN02 — Orçamento deve estar vinculado a um cliente **ativo**.
- RN03 — Não é permitido criar orçamento para cliente inativo.
- RN04 — Pode haver mais de um orçamento em aberto para o mesmo cliente.
- RN05 — Orçamento não tem prazo de validade/expiração.
- RN06 — Orçamento pode conter uma ou mais fichas técnicas.
- RN07 — Custo "Ingredientes" = soma do custo das fichas técnicas × qtd. convidados.
- RN08 — "Custo Total" = Ingredientes + Equipe + Custos Operacionais + Custos Diversos.
- RN09 — "Custo Ajustado" = Custo Total + Margem de Segurança (%).
- RN10 — "Preço Final" = Custo Ajustado + Margem de Lucro (%).
- RN11 — "Valor Por Pessoa" = Preço Final ÷ qtd. convidados.
- RN12 — Margens têm padrão (10%/30%) mas são editáveis por orçamento.
- RN13 — Situação deve ser: Aberto, Enviado ou Aprovado.
- RN14 — Orçamento aprovado pode ser convertido em contrato (ver Estória 12).

---

## Estória 07 — Gerenciar Contratos

**Estória**: Como administrador ou vendedor/comercial, quero gerar um
contrato a partir de um orçamento aprovado, com valor de adiantamento
calculado automaticamente e controle de status de assinatura, para
formalizar o fechamento do evento com o cliente.

### Campos
- Número do contrato (sequencial, automático), Data de emissão, Forma de
  pagamento, Condições/cláusulas (texto), Percentual de adiantamento (%).
- Valor do adiantamento = calculado automaticamente.
- Situação: Aguardando Assinatura, Assinado, Cancelado.

### Regras de negócio
- RN01 — Administrador e Vendedor/Comercial podem gerenciar contratos.
- RN02 — Contrato vinculado a exatamente um orçamento aprovado.
- RN03 — Não permitir mais de um contrato para o mesmo orçamento (1:1).
- RN04 — Valor do adiantamento = percentual de adiantamento × valor total do contrato.
- RN05 — Situação: Aguardando Assinatura, Assinado ou Cancelado.
- RN06 — Contrato assinado dispara criação do Evento e das Contas a Receber
  automáticas (ver módulo Operação/Financeiro).
- RN07 — Cancelar contrato já assinado exige reautenticação (senha) do
  usuário logado.
- RN08 — Contratos não são excluídos fisicamente; cancelamento preserva
  histórico e vínculos gerados.
- RN09 — [Pendência] Geração/coleta de assinatura eletrônica via integração
  externa — plataforma ainda em definição.
- RN10 — [Pendência] Regras de edição pós-assinatura dependem da plataforma
  de assinatura eletrônica escolhida.

---

## Estória 12 — Conversão de Orçamento Aprovado em Contrato

**Estória**: Como administrador ou vendedor/comercial, quero converter um
orçamento aprovado em contrato através de uma ação explícita, para
formalizar o fechamento do evento com os dados corretos do orçamento no
momento da aprovação.

### Comportamento
- Botão "Gerar Contrato" disponível só em orçamentos com situação **Aprovado**.
- Ao acionar, validar: contrato duplicado? cliente ativo? dados obrigatórios
  preenchidos?
- Copiar como **snapshot** (fixo, não reatualiza com edições futuras):
  Cliente, Valor total (Preço Final), Data/hora e local do evento,
  Quantidade de convidados.
- Gerar número do contrato sequencialmente.
- Usuário completa campos próprios do contrato no momento da conversão
  (forma de pagamento, condições, % adiantamento).
- Orçamento original continua editável após a geração do contrato.
- Qualquer alteração no orçamento pós-conversão gera um **Adendo de
  Contrato** (descrição da alteração + data).

### Regras de negócio
- RN01 — Conversão só por ação explícita do usuário, nunca automática.
- RN02 — Só orçamentos "Aprovado" podem virar contrato.
- RN03 — Não permitir mais de um contrato por orçamento.
- RN04 — Cliente do orçamento deve estar ativo no momento da conversão.
- RN05 — Dados copiados são snapshot fixo.
- RN06 — Número do contrato gerado automaticamente, sequencial.
- RN07 — Alteração no orçamento pós-conversão gera Adendo vinculado ao contrato.
- RN08 — [Pendência] Fluxo completo de Adendos (aprovação, impacto no valor)
  será detalhado se necessário.
