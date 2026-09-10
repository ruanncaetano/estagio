namespace tcc.Models.Domain;

/// <summary>
/// Dados exclusivos de um cliente Pessoa Física (tabela <c>cliente_pf</c>,
/// PK = FK para <c>cliente</c>). Faz parte do agregado <see cref="Cliente"/>
/// — não é carregado isoladamente.
/// </summary>
public sealed class ClientePf
{
    /// <summary>Mesma identidade do <see cref="Cliente"/> dono (PK = FK).</summary>
    public int IdCliente { get; set; }

    /// <summary>
    /// CPF só com dígitos (11), sem máscara. Opcional (E01 RN03); quando
    /// informado, precisa ser válido e único entre clientes ativos (E01 RN05).
    /// </summary>
    public string? Cpf { get; set; }

    /// <summary>RG, texto livre. Opcional.</summary>
    public string? Rg { get; set; }
}
