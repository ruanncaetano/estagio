using System.ComponentModel.DataAnnotations;

namespace tcc.Models.Dtos;

/// <summary>
/// Bloco de endereço aninhado nos requests de cliente. Todos os campos são
/// opcionais; se todos vierem vazios/nulos, o cliente fica sem endereço
/// (nenhuma linha em <c>endereco</c>). Os <c>[StringLength]</c> batem com o
/// tamanho das colunas da tabela <c>endereco</c>.
/// </summary>
public sealed class EnderecoRequest
{
    /// <summary>Logradouro / rua.</summary>
    [StringLength(255)]
    public string? Rua { get; set; }

    /// <summary>Número (texto — aceita "s/n").</summary>
    [StringLength(20)]
    public string? Numero { get; set; }

    /// <summary>Bairro.</summary>
    [StringLength(120)]
    public string? Bairro { get; set; }

    /// <summary>Cidade.</summary>
    [StringLength(120)]
    public string? Cidade { get; set; }

    /// <summary>CEP.</summary>
    [StringLength(9)]
    public string? Cep { get; set; }
}
