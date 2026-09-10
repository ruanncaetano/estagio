using System.ComponentModel.DataAnnotations;

namespace tcc.Models.Dtos;

/// <summary>
/// Corpo do PUT de edição de cliente. Mesmos campos do
/// <see cref="CriarClienteRequest"/>, <b>menos o tipo</b>: PF não vira PJ (nem
/// o contrário) por edição — decisão de modelagem (o ERS não prevê troca de
/// tipo; se for preciso, cria-se um cliente novo e inativa-se o antigo).
/// Os campos da especialização que não correspondem ao tipo do cliente são
/// ignorados pelo <c>ClienteService</c>. Ver <see cref="CriarClienteRequest"/>
/// para a divisão de responsabilidade DataAnnotations x Service.
/// </summary>
public sealed class AtualizarClienteRequest
{
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

    /// <summary>
    /// Endereço (bloco aninhado). Enviar todos os campos vazios/nulos remove o
    /// endereço atual do cliente.
    /// </summary>
    public EnderecoRequest? Endereco { get; set; }

    // --- Só PF ---

    /// <summary>CPF, com ou sem máscara (guardado só com dígitos). Opcional (E01 RN03).</summary>
    [StringLength(14, ErrorMessage = "CPF longo demais (máx. 14 caracteres com máscara)")]
    public string? Cpf { get; set; }

    /// <summary>RG. Opcional. Só PF.</summary>
    [StringLength(20)]
    public string? Rg { get; set; }

    // --- Só PJ ---

    /// <summary>CNPJ, com ou sem máscara (guardado só com dígitos). Opcional (E01 RN04).</summary>
    [StringLength(18, ErrorMessage = "CNPJ longo demais (máx. 18 caracteres com máscara)")]
    public string? Cnpj { get; set; }

    /// <summary>Nome fantasia. Opcional. Só PJ.</summary>
    [StringLength(255)]
    public string? NomeFantasia { get; set; }

    /// <summary>Responsável pela conta na empresa. Opcional. Só PJ.</summary>
    [StringLength(255)]
    public string? NomeResponsavel { get; set; }

    /// <summary>Inscrição estadual. Opcional. Só PJ.</summary>
    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }
}
