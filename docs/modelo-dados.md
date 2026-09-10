# Modelo de Dados — DER

> Convertido a partir dos diagramas ER (imagens) do ERS, seção 3.2 e dos
> diagramas de contexto de cada estória. **Validar contra as imagens
> originais do PDF ao implementar** — esta conversão pode ter perdido
> detalhes finos de tipo/tamanho de coluna que não estavam legíveis no
> texto extraído.
>
> **Notas de implementação (Estória 01):** `cpf`/`cnpj` marcados `UK` aqui
> **não** viram `UNIQUE` no banco — a unicidade é só entre clientes *ativos*
> (RN05) e é validada no `ClienteService` (ver `docs/decisions.md`, entrada
> de 2026-09-09). `CLIENTE_PJ.nome_fantasia` foi adicionado a partir do ERS
> (`comercial.md`). O endereço, que o ERS trazia como colunas soltas em cada
> cadastro, virou a entidade `ENDERECO` (FK opcional `id_endereco`),
> reaproveitada por Cliente/Fornecedor/Funcionário — decisão de 2026-09-09.

```mermaid
erDiagram
    CLIENTE ||--o| CLIENTE_PF : "especializa"
    CLIENTE ||--o| CLIENTE_PJ : "especializa"
    CLIENTE {
        int id_cliente PK
        string tipo_cliente
        string nome
        string telefone
        string email
        int id_endereco FK
        boolean ativo
    }
    ENDERECO {
        int id_endereco PK
        string rua
        string numero
        string bairro
        string cidade
        string cep
    }
    CLIENTE ||--o| ENDERECO : "tem"
    FORNECEDOR ||--o| ENDERECO : "tem"
    FUNCIONARIO ||--o| ENDERECO : "tem"
    CLIENTE_PF {
        int id_cliente PK_FK
        string cpf UK
        string rg
    }
    CLIENTE_PJ {
        int id_cliente PK_FK
        string cnpj UK
        string nome_fantasia
        string nome_responsavel
        string inscricao_estadual
    }

    FORNECEDOR {
        int id_fornecedor PK
        string nome
        string cnpj_cpf UK
        string email
        string telefone
        int id_endereco FK
        boolean ativo
    }

    INSUMO {
        int id_insumo PK
        string codigo
        string nome
        string categoria
        string unidade_medida
        boolean ativo
    }
    INSUMO_FORNECEDOR_PRECO {
        int id_preco PK
        int id_insumo FK
        int id_fornecedor FK
        decimal preco
        timestamp data_cadastro
    }
    INSUMO ||--o{ INSUMO_FORNECEDOR_PRECO : "tem precos por"
    FORNECEDOR ||--o{ INSUMO_FORNECEDOR_PRECO : "fornece"

    FICHA_TECNICA {
        int id_ficha_tecnica PK
        string nome_receita
        string descricao
        decimal rendimento_total
        string unidade_rendimento
        int pessoas_atendidas
        int tempo_preparo_min
        int antecedencia_min
        string instrucoes_preparo
        decimal custo_total
        boolean ativo
    }
    FICHA_TECNICA_INSUMO {
        int id_ficha_insumo PK
        int id_ficha_tecnica FK
        int id_insumo FK
        decimal quantidade
        decimal preco_unitario
        decimal subtotal
    }
    FICHA_TECNICA ||--o{ FICHA_TECNICA_INSUMO : "composta por"
    INSUMO ||--o{ FICHA_TECNICA_INSUMO : "usado em"

    EQUIPAMENTO {
        int id_equipamento PK
        string nome
        string categoria
        int quantidade_estoque
        boolean ativo
    }

    FUNCIONARIO {
        int id_funcionario PK
        string nome
        string cpf UK
        string telefone
        string email
        int id_endereco FK
        string funcao_cargo
        decimal valor_referencia
        string forma_pagamento
        string observacao
        boolean ativo
    }

    ORCAMENTO {
        int id_orcamento PK
        int id_cliente FK
        timestamp data_hora_evento
        string local_evento
        int qtd_convidados
        string observacoes
        decimal margem_seguranca
        decimal margem_lucro
        decimal custo_total
        decimal custo_ajustado
        decimal preco_final
        decimal valor_por_pessoa
        string situacao
    }
    CLIENTE ||--o{ ORCAMENTO : "solicita"
    ORCAMENTO_FICHA_TECNICA {
        int id_orcamento_ficha PK
        int id_orcamento FK
        int id_ficha_tecnica FK
    }
    ORCAMENTO ||--o{ ORCAMENTO_FICHA_TECNICA : "inclui"
    FICHA_TECNICA ||--o{ ORCAMENTO_FICHA_TECNICA : "compoe"
    ORCAMENTO_EQUIPE {
        int id_equipe PK
        int id_orcamento FK
        int id_funcionario FK
        int quantidade
        decimal valor_diaria
        decimal subtotal
    }
    ORCAMENTO ||--o{ ORCAMENTO_EQUIPE : "aloca"
    FUNCIONARIO ||--o{ ORCAMENTO_EQUIPE : "participa de"
    ORCAMENTO_CUSTO_OPERACIONAL {
        int id_custo_operacional PK
        int id_orcamento FK
        decimal transporte
        decimal gas
        decimal energia
        decimal logistica
        decimal outros
    }
    ORCAMENTO ||--o| ORCAMENTO_CUSTO_OPERACIONAL : "tem"
    ORCAMENTO_CUSTO_DIVERSO {
        int id_custo_diverso PK
        int id_orcamento FK
        string descricao
        decimal valor
    }
    ORCAMENTO ||--o{ ORCAMENTO_CUSTO_DIVERSO : "tem"

    CONTRATO {
        int id_contrato PK
        int id_orcamento FK, UK
        string numero_contrato
        date data_emissao
        string forma_pagamento
        string condicoes
        decimal percentual_adiantamento
        decimal valor_adiantamento
        decimal valor_total
        int numero_parcelas_restante
        string situacao
        timestamp data_assinatura
        timestamp data_cancelamento
    }
    ORCAMENTO ||--o| CONTRATO : "gera (1:1)"
    ADENDO_CONTRATO {
        int id_adendo PK
        int id_contrato FK
        string descricao_alteracao
        date data
    }
    CONTRATO ||--o{ ADENDO_CONTRATO : "registra"

    EVENTO {
        int id_evento PK
        int id_contrato FK, UK
        int id_cliente FK
        timestamp data_hora_evento
        string local_evento
        int qtd_convidados
        string situacao
    }
    CONTRATO ||--o| EVENTO : "gera (1:1, ao assinar)"
    CLIENTE ||--o{ EVENTO : "participa de"

    FICHA_PRODUCAO {
        int id_ficha_producao PK
        int id_evento FK
        string situacao
    }
    EVENTO ||--o| FICHA_PRODUCAO : "gera"
    FICHA_PRODUCAO_ITEM {
        int id_ficha_producao_item PK
        int id_ficha_producao FK
        int id_ficha_tecnica FK
        decimal fator_escala
    }
    FICHA_PRODUCAO ||--o{ FICHA_PRODUCAO_ITEM : "escala"
    FICHA_TECNICA ||--o{ FICHA_PRODUCAO_ITEM : "usada em"
    FICHA_PRODUCAO_ITEM_INSUMO {
        int id_item_insumo PK
        int id_ficha_producao_item FK
        int id_insumo FK
        decimal quantidade_necessaria
    }
    FICHA_PRODUCAO_ITEM ||--o{ FICHA_PRODUCAO_ITEM_INSUMO : "consolida"
    FICHA_PRODUCAO_EQUIPE {
        int id_ficha_producao_equipe PK
        int id_ficha_producao FK
        int id_funcionario FK
        int quantidade
    }
    FICHA_PRODUCAO ||--o{ FICHA_PRODUCAO_EQUIPE : "aloca"
    FUNCIONARIO ||--o{ FICHA_PRODUCAO_EQUIPE : "escalado em"
    FICHA_PRODUCAO_EQUIPAMENTO {
        int id_ficha_producao_equipamento PK
        int id_ficha_producao FK
        int id_equipamento FK
        int quantidade_reservada
    }
    FICHA_PRODUCAO ||--o{ FICHA_PRODUCAO_EQUIPAMENTO : "reserva"
    EQUIPAMENTO ||--o{ FICHA_PRODUCAO_EQUIPAMENTO : "reservado em"

    CONTA_PAGAR {
        int id_conta_pagar PK
        int id_fornecedor FK
        int id_funcionario FK
        timestamp data_lancamento
        string descricao
        decimal valor
        date data_vencimento
        string categoria
        int numero_parcela
        int total_parcelas
        string situacao
    }
    FORNECEDOR ||--o{ CONTA_PAGAR : "recebe"
    FUNCIONARIO ||--o{ CONTA_PAGAR : "recebe"
    PAGAMENTO_CONTA_PAGAR {
        int id_pagamento PK
        int id_conta_pagar FK
        date data_pagamento
        string forma_pagamento
        decimal valor_pago
    }
    CONTA_PAGAR ||--o{ PAGAMENTO_CONTA_PAGAR : "recebe"

    CONTA_RECEBER {
        int id_conta_receber PK
        int id_cliente FK
        int id_contrato FK
        timestamp data_lancamento
        string descricao
        decimal valor
        date data_vencimento
        string categoria
        boolean eh_sinal
        int numero_parcela
        int total_parcelas
        string situacao
    }
    CLIENTE ||--o{ CONTA_RECEBER : "gera"
    CONTRATO ||--o{ CONTA_RECEBER : "origina"
    RECEBIMENTO_CONTA_RECEBER {
        int id_recebimento PK
        int id_conta_receber FK
        date data_recebimento
        string forma_recebimento
        decimal valor_recebido
    }
    CONTA_RECEBER ||--o{ RECEBIMENTO_CONTA_RECEBER : "recebe"

    SOCIO {
        int id_socio PK
        string nome
        decimal percentual_participacao
    }
```
