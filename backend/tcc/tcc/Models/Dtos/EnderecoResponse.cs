using tcc.Models.Domain;

namespace tcc.Models.Dtos;

/// <summary>Endereço do cliente devolvido pela API. Vem <c>null</c> se o cliente não tem endereço.</summary>
public sealed class EnderecoResponse
{
    /// <summary>Logradouro / rua.</summary>
    public string? Rua { get; set; }

    /// <summary>Número do endereço.</summary>
    public string? Numero { get; set; }

    /// <summary>Bairro.</summary>
    public string? Bairro { get; set; }

    /// <summary>Cidade.</summary>
    public string? Cidade { get; set; }

    /// <summary>CEP.</summary>
    public string? Cep { get; set; }

    /// <summary>Monta o DTO a partir da entidade, ou <c>null</c> se não houver endereço.</summary>
    public static EnderecoResponse? DoDominio(Endereco? e)
    {
        if (e is null || e.Vazio)
        {
            return null;
        }

        return new EnderecoResponse
        {
            Rua = e.Rua,
            Numero = e.Numero,
            Bairro = e.Bairro,
            Cidade = e.Cidade,
            Cep = e.Cep,
        };
    }
}
