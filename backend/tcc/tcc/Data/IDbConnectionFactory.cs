using MySqlConnector;

namespace tcc.Data;

/// <summary>
/// Cria conexões com o MySQL para os Repositories. Abstraído numa interface
/// para o Repository não depender de como a connection string é obtida e para
/// facilitar testes.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Abre e devolve uma conexão pronta para uso. Quem chama é dono da
    /// conexão e deve descartá-la (<c>await using</c>).
    /// </summary>
    Task<MySqlConnection> AbrirAsync(CancellationToken cancellationToken = default);
}
