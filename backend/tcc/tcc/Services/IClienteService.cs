using tcc.Common;
using tcc.Models.Dtos;

namespace tcc.Services;

/// <summary>
/// Regras de negócio da Estória 01 — Gerenciar Clientes
/// (<c>docs/requisitos/comercial.md</c>). O Controller só chama estes métodos
/// e traduz o <see cref="Result"/> para HTTP.
/// </summary>
public interface IClienteService
{
    /// <summary>Lista clientes aplicando os filtros de situação e busca.</summary>
    Task<IReadOnlyList<ClienteResponse>> ListarAsync(bool? ativo, string? busca, CancellationToken cancellationToken = default);

    /// <summary>Obtém um cliente por id. Falha <see cref="TipoFalha.NaoEncontrado"/> se não existir.</summary>
    Task<Result<ClienteResponse>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um cliente PF ou PJ. Valida E01 RN02 (tipo), RN03/RN04 (documento
    /// opcional e bem formado) e RN05 (documento único entre ativos).
    /// </summary>
    Task<Result<ClienteResponse>> CriarAsync(CriarClienteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Edita os dados de um cliente existente (o tipo PF/PJ não muda). Mesmas
    /// validações de documento da criação.
    /// </summary>
    Task<Result<ClienteResponse>> AtualizarAsync(int id, AtualizarClienteRequest request, CancellationToken cancellationToken = default);

    /// <summary>Inativa o cliente (E01 RN06/RN07). Não apaga nada (E01 RN08).</summary>
    Task<Result> InativarAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reativa um cliente inativo. Reexecuta E01 RN05 para não deixar dois
    /// clientes ativos com o mesmo documento.
    /// </summary>
    Task<Result> ReativarAsync(int id, CancellationToken cancellationToken = default);
}
