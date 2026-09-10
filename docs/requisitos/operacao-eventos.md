# Módulo Operação / Eventos

Estórias: 06 (Equipamentos), 10 (Funcionários), 13 (Conversão Contrato→Evento),
14 (Ficha de Produção do Evento).

---

## Estória 06 — Gerenciar Equipamentos

**Estória**: Como administrador ou vendedor/comercial, quero cadastrar,
consultar, editar e inativar equipamentos, com controle da quantidade total
em estoque, para manter um catálogo confiável dos equipamentos disponíveis
para uso em eventos.

### Campos
- Nome, Categoria/tipo, Quantidade total em estoque (próprio).

### Regras de negócio
- RN01 — Administrador e Vendedor/Comercial podem cadastrar/gerenciar.
- RN02 — Permitir equipamentos com nomes duplicados.
- RN03 — Quantidade = total do estoque próprio da empresa, sem cálculo de
  reserva/disponibilidade por evento (isso é tratado na gestão do evento).
- RN04 — Não excluir fisicamente.
- RN05 — Deve ter situação ativo/inativo.
- RN06 — Histórico e vínculos (uso em eventos anteriores) preservados após
  inativação.

---

## Estória 10 — Gerenciar Funcionários

**Estória**: Como administrador, quero cadastrar, consultar, editar e
inativar funcionários (colaboradores freelancer recorrentes), para manter
uma base organizada de mão de obra, utilizada na composição de orçamentos,
eventos e geração de contas a pagar.

### Campos
- Nome, CPF (opcional), Telefone, E-mail, Endereço, Função/cargo, Valor de
  referência (diária), Forma de pagamento, Observação.

### Comportamento
- Funcionários ativos ficam disponíveis pra seleção na composição de
  Equipe/Mão de Obra de orçamentos e eventos, trazendo o valor de referência
  como sugestão editável naquele contexto.

### Regras de negócio
- RN01 — Apenas Administrador cadastra/gerencia funcionários.
- RN02 — CPF não é obrigatório.
- RN03 — Quando informado, CPF não pode se repetir, mesmo entre um ativo e
  um inativo.
- RN04 — Valor de referência é sugerido automaticamente ao selecionar o
  funcionário, mas pode ser ajustado manualmente naquele contexto.
- RN05 — Não excluir fisicamente.
- RN06 — Deve ter situação ativo/inativo.
- RN07 — Histórico (participação em eventos, contas a pagar geradas)
  preservado após inativação.
- RN08 — Apenas funcionários ativos podem ser selecionados em novos
  orçamentos ou eventos.

---

## Estória 13 — Conversão de Contrato Assinado em Evento

**Estória**: Como sistema, quero gerar automaticamente um Evento assim que
um contrato for marcado como Assinado, copiando os dados do contrato, para
disparar a geração da Ficha de Produção e das movimentações financeiras
automáticas do evento.

### Comportamento
- Disparado automaticamente pela mudança de situação do contrato para
  "Assinado" — **não é ação manual**.
- Copia para o Evento: Cliente, Data/hora do evento, Local do evento,
  Quantidade de convidados.
- Situação inicial do Evento: **Agendado**. Demais situações possíveis: Em
  Andamento, Concluído, Cancelado.
- A geração do Evento dispara, também automaticamente:
  - Criação da Ficha de Produção (agora vinculada ao Evento, não mais ao Orçamento).
  - Geração automática de Contas a Receber (ver Estória 15).
  - Geração automática de Contas a Pagar por funcionário vinculado (ver Estória 16).

### Regras de negócio
- RN01 — Evento gerado automaticamente pelo sistema, sem ação manual, no
  momento da assinatura do contrato.
- RN02 — Contrato assinado gera exatamente um Evento (1:1).
- RN03 — Dados do Evento são snapshot do contrato/orçamento no momento da geração.
- RN04 — Evento deve ter situação: Agendado, Em Andamento, Concluído ou Cancelado.
- RN05 — Geração do Evento é o gatilho oficial pra Ficha de Produção e
  movimentações financeiras automáticas.
- RN06 — A partir daqui, a Ficha de Produção é gerada a partir do Evento,
  não mais diretamente do Orçamento.

---

## Estória 14 — Geração e Gestão da Ficha de Produção do Evento

**Estória**: Como administrador ou equipe de cozinha, quero gerar
automaticamente uma ficha de produção a partir de um orçamento (hoje) ou
evento (após a Estória 13 estar pronta), reunindo cardápio escalado, equipe
e equipamentos, com possibilidade de ajuste manual, para ter um painel único
que oriente toda a execução do evento.

### Comportamento
- Geração automática trazendo:
  - Dados do evento: cliente, data/hora, local, quantidade de convidados.
  - Cardápio: fichas técnicas selecionadas.
  - Equipe/mão de obra vinculada.
  - Equipamentos vinculados.
- Todo dado herdado pode ser **editado manualmente** após a geração.
- Recalcular automaticamente a quantidade necessária de cada insumo:
  `quantidade na ficha técnica × (convidados do evento ÷ pessoas atendidas da receita)`.
- Exibir, por prato: instruções de preparo, tempo de preparo, antecedência
  (herdados da Ficha Técnica).
- Consulta e ajuste da equipe/mão de obra e dos equipamentos reservados.
- Situação da ficha: Aguardando Preparo, Em Preparo, Pronta.

### Regras de negócio
- RN01 — Administrador e equipe de Cozinha consultam/gerenciam a Ficha de Produção.
- RN02 — Ficha herda cardápio, quantidade de convidados, equipe e
  equipamentos da origem.
- RN03 — Todos os dados herdados podem ser editados manualmente pós-geração.
- RN04 — Recálculo proporcional de insumo: quantidade na ficha técnica ×
  (convidados do evento ÷ pessoas atendidas da receita).
- RN05 — Situação: Aguardando Preparo, Em Preparo ou Pronta.
- RN06 — [Pendência/transição] A referência de origem será ajustada para
  apontar ao **Evento** quando essa entidade for implementada — atualmente
  o ERS descreve a geração a partir do Orçamento como estado intermediário.
  **Implementar já pensando no Evento como origem final**, já que a Estória
  13 formaliza esse fluxo.
