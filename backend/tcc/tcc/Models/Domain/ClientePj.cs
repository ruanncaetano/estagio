namespace tcc.Models.Domain;

/// <summary>
/// Dados exclusivos de um cliente Pessoa Jurídica (tabela <c>cliente_pj</c>,
/// PK = FK para <c>cliente</c>). Faz parte do agregado <see cref="Cliente"/>.
/// A Razão Social fica em <see cref="Cliente.Nome"/> (campo comum).
/// </summary>
public sealed class ClientePj
{
    /// <summary>Mesma identidade do <see cref="Cliente"/> dono (PK = FK).</summary>
    public int IdCliente { get; set; }

    /// <summary>
    /// CNPJ só com dígitos (14), sem máscara. Opcional (E01 RN04); quando
    /// informado, precisa ser válido e único entre clientes ativos (E01 RN05).
    /// </summary>
    public string? Cnpj { get; set; }

    /// <summary>Nome fantasia. Opcional.</summary>
    public string? NomeFantasia { get; set; }

    /// <summary>Pessoa responsável pela conta na empresa. Opcional.</summary>
    public string? NomeResponsavel { get; set; }

    /// <summary>Inscrição estadual. Opcional.</summary>
    public string? InscricaoEstadual { get; set; }
}
