# Diagrama de Classes

> Convertido a partir do "Diagrama de Classes Final" (seção 3.1 do ERS) e
> dos diagramas de contexto por estória. Reflete a mesma estrutura do
> `docs/modelo-dados.md`, em nível de classe/objeto em vez de tabela —
> **validar contra a imagem original ao implementar**.

```mermaid
classDiagram
    class Cliente {
        int idCliente
        string tipoCliente
        string nome
        string telefone
        string email
        string rua
        string numero
        string bairro
        string cidade
        string cep
        boolean ativo
    }
    class ClientePessoaFisica {
        string cpf
        string rg
    }
    class ClientePessoaJuridica {
        string cnpj
        string inscricaoEstadual
        string nomeResponsavel
    }
    ClientePessoaFisica --|> Cliente
    ClientePessoaJuridica --|> Cliente

    class Fornecedor {
        int idFornecedor
        string nome
        string cnpjCpf
        string email
        string telefone
        string rua
        string numero
        string bairro
        string cidade
        string cep
        boolean ativo
    }

    class Insumo {
        int idInsumo
        string codigo
        string nome
        string categoria
        string unidadeMedida
        boolean ativo
    }
    class InsumoFornecedorPreco {
        int idPreco
        decimal preco
        datetime dataCadastro
    }
    Insumo "1" --> "*" InsumoFornecedorPreco : precos
    Fornecedor "1" --> "*" InsumoFornecedorPreco : fornecedor

    class FichaTecnica {
        int idFichaTecnica
        string nomeReceita
        string descricao
        string unidadeRendimento
        int pessoasAtendidas
        int tempoPreparoMin
        int antecedenciaMin
        string instrucoesPreparo
        decimal custoTotal
        boolean ativo
    }
    class FichaTecnicaInsumo {
        int idFichaInsumo
        decimal quantidade
        decimal precoUnitario
        decimal subtotal
    }
    FichaTecnica "1" --> "*" FichaTecnicaInsumo : ingredientes
    Insumo "1" --> "*" FichaTecnicaInsumo : insumo

    class Equipamento {
        int idEquipamento
        string nome
        string categoria
        int quantidadeEstoque
        boolean ativo
    }

    class Funcionario {
        int idFuncionario
        string nome
        string cpf
        string telefone
        string email
        string rua
        string numero
        string bairro
        string cidade
        string cep
        string funcaoCargo
        decimal valorReferencia
        string formaPagamento
        string observacao
        boolean ativo
    }

    class Orcamento {
        int idOrcamento
        string localEvento
        int qtdConvidados
        string observacoes
        decimal margemSeguranca
        decimal margemLucro
        decimal custoTotal
        decimal custoAjustado
        decimal precoFinal
        decimal valorPorPessoa
        string situacao
    }
    Cliente "1" --> "*" Orcamento : cliente
    class OrcamentoFichaTecnica {
        int idOrcamentoFicha
    }
    Orcamento "1" --> "*" OrcamentoFichaTecnica : itensCardapio
    FichaTecnica "1" --> "*" OrcamentoFichaTecnica : fichaTecnica
    class OrcamentoEquipe {
        int idEquipe
        int quantidade
        decimal valorDiaria
        decimal subtotal
    }
    Orcamento "1" --> "*" OrcamentoEquipe : equipe
    Funcionario "1" --> "*" OrcamentoEquipe : funcionario
    class OrcamentoCustoOperacional {
        int idCustoOperacional
        decimal transporte
        decimal gas
        decimal energia
        decimal logistica
        decimal outros
    }
    Orcamento "1" --> "0..1" OrcamentoCustoOperacional : custosOperacionais
    class OrcamentoCustoDiverso {
        int idCustoDiverso
        string descricao
        decimal valor
    }
    Orcamento "1" --> "*" OrcamentoCustoDiverso : custosDiversos

    class Contrato {
        int idContrato
        string numeroContrato
        date dataEmissao
        string formaPagamento
        string condicoes
        decimal percentualAdiantamento
        decimal valorAdiantamento
        int numeroParcelasRestante
        string situacao
    }
    Orcamento "1" --> "0..1" Contrato : contrato
    class AdendoContrato {
        int idAdendo
        string descricaoAlteracao
        date data
    }
    Contrato "1" --> "*" AdendoContrato : adendos

    class Evento {
        int idEvento
        timestamp dataHoraEvento
        string localEvento
        int qtdConvidados
        string situacao
    }
    Contrato "1" --> "0..1" Evento : evento
    Cliente "1" --> "*" Evento : cliente

    class FichaProducao {
        int idFichaProducao
        string situacao
    }
    Evento "1" --> "0..1" FichaProducao : fichaProducao
    class FichaProducaoItem {
        int idFichaProducaoItem
        decimal fatorEscala
    }
    FichaProducao "1" --> "*" FichaProducaoItem : itens
    FichaTecnica "1" --> "*" FichaProducaoItem : fichaTecnica
    class FichaProducaoItemInsumo {
        int idItemInsumo
        decimal quantidadeNecessaria
    }
    FichaProducaoItem "1" --> "*" FichaProducaoItemInsumo : insumosNecessarios
    class FichaProducaoEquipe {
        int idFichaProducaoEquipe
        int quantidade
    }
    FichaProducao "1" --> "*" FichaProducaoEquipe : equipe
    Funcionario "1" --> "*" FichaProducaoEquipe : funcionario
    class FichaProducaoEquipamento {
        int idFichaProducaoEquipamento
        int quantidadeReservada
    }
    FichaProducao "1" --> "*" FichaProducaoEquipamento : equipamentos
    Equipamento "1" --> "*" FichaProducaoEquipamento : equipamento

    class ContaPagar {
        int idContaPagar
        string descricao
        decimal valor
        date dataVencimento
        string categoria
        int numeroParcela
        int totalParcelas
        string situacao
    }
    Fornecedor "1" --> "*" ContaPagar : contaPagar
    Funcionario "1" --> "*" ContaPagar : contaPagar
    class PagamentoContaPagar {
        int idPagamento
        date dataPagamento
        string formaPagamento
        decimal valorPago
    }
    ContaPagar "1" --> "*" PagamentoContaPagar : pagamentos

    class ContaReceber {
        int idContaReceber
        string descricao
        decimal valor
        date dataVencimento
        string categoria
        boolean ehSinal
        int numeroParcela
        int totalParcelas
        string situacao
    }
    Cliente "1" --> "*" ContaReceber : contaReceber
    Contrato "1" --> "*" ContaReceber : contaReceber
    class RecebimentoContaReceber {
        int idRecebimento
        date dataRecebimento
        string formaRecebimento
        decimal valorRecebido
    }
    ContaReceber "1" --> "*" RecebimentoContaReceber : recebimentos

    class Socio {
        int idSocio
        string nome
        decimal percentualParticipacao
    }
```
