using tcc.Models.Domain;

namespace tcc.Models.Dtos;

/// <summary>
/// Representação de um cliente devolvida pela API. Campos de PF e de PJ
/// convivem no mesmo objeto; só o bloco correspondente a <see cref="Tipo"/>
/// vem preenchido, o outro fica <c>null</c>.
/// </summary>
public sealed class ClienteResponse
{
    /// <summary>Identificador do cliente.</summary>
    public int IdCliente { get; set; }

    /// <summary>"PF" ou "PJ".</summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Nome (PF) ou Razão Social (PJ).</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>WhatsApp / telefone.</summary>
    public string? Telefone { get; set; }

    /// <summary>E-mail.</summary>
    public string? Email { get; set; }

    /// <summary>Endereço (bloco aninhado). <c>null</c> se o cliente não tem endereço cadastrado.</summary>
    public EnderecoResponse? Endereco { get; set; }

    /// <summary><c>true</c> = ativo; <c>false</c> = inativado (E01 RN07).</summary>
    public bool Ativo { get; set; }

    // --- Preenchidos só quando Tipo = "PF" ---

    /// <summary>CPF só com dígitos (sem máscara). <c>null</c> se não informado ou se PJ.</summary>
    public string? Cpf { get; set; }

    /// <summary>RG. <c>null</c> se PJ.</summary>
    public string? Rg { get; set; }

    // --- Preenchidos só quando Tipo = "PJ" ---

    /// <summary>CNPJ só com dígitos (sem máscara). <c>null</c> se não informado ou se PF.</summary>
    public string? Cnpj { get; set; }

    /// <summary>Nome fantasia. <c>null</c> se PF.</summary>
    public string? NomeFantasia { get; set; }

    /// <summary>Responsável pela conta na empresa. <c>null</c> se PF.</summary>
    public string? NomeResponsavel { get; set; }

    /// <summary>Inscrição estadual. <c>null</c> se PF.</summary>
    public string? InscricaoEstadual { get; set; }

    /// <summary>Monta o DTO de resposta a partir da entidade de domínio.</summary>
    public static ClienteResponse DoDominio(Cliente c) => new()
    {
        IdCliente = c.IdCliente,
        Tipo = c.Tipo.ParaTexto(),
        Nome = c.Nome,
        Telefone = c.Telefone,
        Email = c.Email,
        Endereco = EnderecoResponse.DoDominio(c.Endereco),
        Ativo = c.Ativo,
        Cpf = c.Pf?.Cpf,
        Rg = c.Pf?.Rg,
        Cnpj = c.Pj?.Cnpj,
        NomeFantasia = c.Pj?.NomeFantasia,
        NomeResponsavel = c.Pj?.NomeResponsavel,
        InscricaoEstadual = c.Pj?.InscricaoEstadual,
    };
}
