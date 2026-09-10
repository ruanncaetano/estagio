using Dapper;
using MySqlConnector;

namespace tcc.Data;

/// <summary>
/// Implementação de <see cref="IDbConnectionFactory"/> que lê a connection
/// string <c>"MySql"</c> da configuração (<c>appsettings.Local.json</c>, fora
/// do git) e abre uma <see cref="MySqlConnection"/>.
/// </summary>
public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    static MySqlConnectionFactory()
    {
        // Colunas do banco são snake_case (id_cliente), propriedades C# são
        // PascalCase (IdCliente). Isso liga o mapeamento automático do Dapper.
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException(
                "Connection string 'MySql' não configurada. Crie backend/tcc/tcc/appsettings.Local.json "
                + "a partir de appsettings.Local.example.json.");
    }

    public async Task<MySqlConnection> AbrirAsync(CancellationToken cancellationToken = default)
    {
        var conexao = new MySqlConnection(_connectionString);
        await conexao.OpenAsync(cancellationToken);
        return conexao;
    }
}
