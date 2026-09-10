using System.ComponentModel.DataAnnotations;

namespace tcc.Models.Dtos;

/// <summary>
/// Corpo do POST de criação de cliente. É um DTO "achatado" que acomoda PF e
/// PJ: <see cref="Tipo"/> decide quais campos abaixo são usados — os demais
/// são ignorados. As regras de qual campo vale para qual tipo estão no
/// <c>ClienteService</c> (E01 RN02/RN03/RN04).
///
/// Os atributos <c>DataAnnotations</c> aqui cuidam do "shape" da entrada
/// (obrigatoriedade, tamanho, formato bruto) — com <c>[ApiController]</c> uma
/// violação vira <c>400 ValidationProblemDetails</c> automático. Regras que
/// dependem de lógica/banco (dígito verificador, unicidade E01 RN05) ficam no
/// Service via <c>Result</c>.
///
/// Documento: aceitamos CPF/CNPJ com ou sem máscara e limpamos no Service —
/// por isso o <c>[StringLength]</c> comporta a versão mascarada; a contagem
/// exata de dígitos é validada no <c>ClienteService</c>.
/// </summary>
public sealed class CriarClienteRequest
{
    /// <summary>"PF" ou "PJ" (E01 RN02). Obrigatório e imutável após a criação.</summary>
    [Required]
    [RegularExpression("^(?i)(PF|PJ)$", ErrorMessage = "Tipo deve ser PF ou PJ")]
    public string? Tipo { get; set; }

    /// <summary>Nome (PF) ou Razão Social (PJ). Obrigatório.</summary>
    [Required]
    [StringLength(255)]
    public string? Nome { get; set; }

    /// <summary>WhatsApp / telefone. Opcional.</summary>
    [StringLength(20)]
    public string? Telefone { get; set; }

    /// <summary>E-mail. Opcional.</summary>
    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    /// <summary>Endereço (bloco aninhado). Opcional — omitir ou enviar vazio deixa o cliente sem endereço.</summary>
    public EnderecoRequest? Endereco { get; set; }

    // --- Só PF ---

    /// <summary>CPF, com ou sem máscara (o Service guarda só dígitos). Opcional (E01 RN03).</summary>
    [StringLength(14, ErrorMessage = "CPF longo demais (máx. 14 caracteres com máscara)")]
    public string? Cpf { get; set; }

    /// <summary>RG. Opcional. Usado só quando <see cref="Tipo"/> = "PF".</summary>
    [StringLength(20)]
    public string? Rg { get; set; }

    // --- Só PJ ---

    /// <summary>CNPJ, com ou sem máscara (o Service guarda só dígitos). Opcional (E01 RN04).</summary>
    [StringLength(18, ErrorMessage = "CNPJ longo demais (máx. 18 caracteres com máscara)")]
    public string? Cnpj { get; set; }

    /// <summary>Nome fantasia. Opcional. Usado só quando <see cref="Tipo"/> = "PJ".</summary>
    [StringLength(255)]
    public string? NomeFantasia { get; set; }

    /// <summary>Responsável pela conta na empresa. Opcional. Só PJ.</summary>
    [StringLength(255)]
    public string? NomeResponsavel { get; set; }

    /// <summary>Inscrição estadual. Opcional. Só PJ.</summary>
    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }
}
