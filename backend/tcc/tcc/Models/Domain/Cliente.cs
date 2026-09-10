namespace tcc.Models.Domain;

/// <summary>
/// Cliente do buffet — raiz do agregado. Reúne os campos comuns (tabela
/// <c>cliente</c>) e, conforme <see cref="Tipo"/>, exatamente um dos blocos
/// de especialização (<see cref="Pf"/> ou <see cref="Pj"/>). O endereço é uma
/// entidade própria (<see cref="Domain.Endereco"/>) referenciada por FK opcional.
/// </summary>
public sealed class Cliente
{
    /// <summary>Identidade (coluna <c>id_cliente</c>, auto-incremento).</summary>
    public int IdCliente { get; set; }

    /// <summary>PF ou PJ. Não muda depois de criado (ver DTOs).</summary>
    public TipoCliente Tipo { get; set; }

    /// <summary>Nome (PF) ou Razão Social (PJ). Obrigatório.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>WhatsApp / telefone de contato. Opcional.</summary>
    public string? Telefone { get; set; }

    /// <summary>E-mail de contato. Opcional.</summary>
    public string? Email { get; set; }

    /// <summary>
    /// Endereço do cliente (tabela <c>endereco</c>, FK <c>cliente.id_endereco</c>).
    /// <c>null</c> quando nenhum campo de endereço foi informado.
    /// </summary>
    public Endereco? Endereco { get; set; }

    /// <summary>
    /// E01 RN07 — situação ativo/inativo. E01 RN06 — inativar em vez de
    /// excluir fisicamente. <c>true</c> = ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Bloco PF. Preenchido somente quando <see cref="Tipo"/> = Pf.</summary>
    public ClientePf? Pf { get; set; }

    /// <summary>Bloco PJ. Preenchido somente quando <see cref="Tipo"/> = Pj.</summary>
    public ClientePj? Pj { get; set; }

    /// <summary>
    /// Documento fiscal do cliente só com dígitos (CPF se PF, CNPJ se PJ),
    /// ou <c>null</c> se não informado. Atalho usado pela validação de
    /// unicidade (E01 RN05).
    /// </summary>
    public string? Documento => Tipo == TipoCliente.Pf ? Pf?.Cpf : Pj?.Cnpj;
}
