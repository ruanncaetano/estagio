using tcc.Models.Domain;

namespace tcc.Repositories;

/// <summary>
/// Acesso a dados de <see cref="Cliente"/> (tabelas <c>cliente</c> +
/// <c>cliente_pf</c> / <c>cliente_pj</c>). Sem <c>DELETE</c> físico
/// (E01 RN06): a "remoção" é feita por <see cref="DefinirAtivoAsync"/>.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Lista clientes com o bloco PF/PJ já carregado.
    /// </summary>
    /// <param name="ativo">
    /// <c>null</c> = todos; <c>true</c> = só ativos; <c>false</c> = só inativos.
    /// </param>
    /// <param name="busca">
    /// Trecho para casar por nome/razão social (LIKE) ou por documento
    /// (CPF/CNPJ, só dígitos). <c>null</c>/vazio = sem filtro de texto.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    Task<IReadOnlyList<Cliente>> ListarAsync(bool? ativo, string? busca, CancellationToken cancellationToken = default);

    /// <summary>Obtém um cliente por id, ou <c>null</c> se não existir.</summary>
    Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Insere o cliente e o bloco de especialização (PF ou PJ) numa única
    /// transação. Devolve o <c>id_cliente</c> gerado.
    /// </summary>
    Task<int> InserirAsync(Cliente cliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza os campos comuns e os do bloco de especialização numa única
    /// transação. Não altera <c>tipo_cliente</c> nem <c>ativo</c>.
    /// </summary>
    Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Liga/desliga a flag <c>ativo</c> (E01 RN06/RN07). Nunca apaga linha
    /// nem toca nas tabelas filhas — o histórico fica intacto (E01 RN08).
    /// Devolve <c>false</c> se o id não existe.
    /// </summary>
    Task<bool> DefinirAtivoAsync(int id, bool ativo, CancellationToken cancellationToken = default);

    /// <summary>
    /// E01 RN05 (helper) — existe algum cliente <b>ativo</b> com este CPF,
    /// desconsiderando <paramref name="idExcluir"/> (usado no update para
    /// ignorar o próprio registro)?
    /// </summary>
    Task<bool> ExisteAtivoComCpfAsync(string cpf, int? idExcluir, CancellationToken cancellationToken = default);

    /// <summary>
    /// E01 RN05 (helper) — existe algum cliente <b>ativo</b> com este CNPJ,
    /// desconsiderando <paramref name="idExcluir"/>?
    /// </summary>
    Task<bool> ExisteAtivoComCnpjAsync(string cnpj, int? idExcluir, CancellationToken cancellationToken = default);
}
