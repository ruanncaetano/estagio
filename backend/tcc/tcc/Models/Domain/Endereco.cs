namespace tcc.Models.Domain;

/// <summary>
/// Endereço postal. Entidade própria (tabela <c>endereco</c>) reaproveitada
/// por Cliente, Fornecedor e Funcionário — cada dono referencia o seu por
/// <c>id_endereco</c> (FK opcional, 1:1, não compartilhado).
/// Todos os campos são opcionais.
/// </summary>
public sealed class Endereco
{
    /// <summary>Identidade (coluna <c>id_endereco</c>, auto-incremento). 0 enquanto não persistido.</summary>
    public int IdEndereco { get; set; }

    /// <summary>Logradouro / rua.</summary>
    public string? Rua { get; set; }

    /// <summary>Número (texto — aceita "s/n").</summary>
    public string? Numero { get; set; }

    /// <summary>Bairro.</summary>
    public string? Bairro { get; set; }

    /// <summary>Cidade.</summary>
    public string? Cidade { get; set; }

    /// <summary>CEP.</summary>
    public string? Cep { get; set; }

    /// <summary><c>true</c> quando todos os campos estão vazios — nesse caso não vale a pena gravar a linha.</summary>
    public bool Vazio =>
        string.IsNullOrWhiteSpace(Rua)
        && string.IsNullOrWhiteSpace(Numero)
        && string.IsNullOrWhiteSpace(Bairro)
        && string.IsNullOrWhiteSpace(Cidade)
        && string.IsNullOrWhiteSpace(Cep);
}
