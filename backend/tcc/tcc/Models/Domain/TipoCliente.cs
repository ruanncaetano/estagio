namespace tcc.Models.Domain;

/// <summary>
/// Discrimina a especialização do cliente (herança table-per-type:
/// <c>cliente_pf</c> / <c>cliente_pj</c>). Persistido na coluna
/// <c>cliente.tipo_cliente</c> como a string "PF" ou "PJ" — a conversão
/// enum &lt;-&gt; string fica em <see cref="TipoClienteExtensions"/>.
/// E01 RN02 — o sistema aceita Pessoa Física e Pessoa Jurídica.
/// </summary>
public enum TipoCliente
{
    /// <summary>Pessoa Física — dados extras em <c>cliente_pf</c> (CPF, RG).</summary>
    Pf = 1,

    /// <summary>Pessoa Jurídica — dados extras em <c>cliente_pj</c> (CNPJ, razão/fantasia...).</summary>
    Pj = 2,
}

/// <summary>
/// Conversão entre <see cref="TipoCliente"/> e o texto "PF"/"PJ" gravado no
/// banco. Centralizado aqui para o Repository e o Service não repetirem o
/// mapeamento.
/// </summary>
public static class TipoClienteExtensions
{
    /// <summary>Texto persistido no banco ("PF" ou "PJ").</summary>
    public static string ParaTexto(this TipoCliente tipo) => tipo switch
    {
        TipoCliente.Pf => "PF",
        TipoCliente.Pj => "PJ",
        _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de cliente desconhecido."),
    };

    /// <summary>
    /// Tenta converter o texto vindo da API/banco para <see cref="TipoCliente"/>.
    /// Aceita "PF"/"PJ" em qualquer caixa. Retorna <c>false</c> se não reconhecer
    /// (o Service traduz isso na validação da E01 RN02).
    /// </summary>
    public static bool TentarConverter(string? texto, out TipoCliente tipo)
    {
        switch (texto?.Trim().ToUpperInvariant())
        {
            case "PF":
                tipo = TipoCliente.Pf;
                return true;
            case "PJ":
                tipo = TipoCliente.Pj;
                return true;
            default:
                tipo = default;
                return false;
        }
    }
}
