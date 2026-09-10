using Dapper;
using MySqlConnector;
using tcc.Data;
using tcc.Models.Domain;

namespace tcc.Repositories;

/// <summary>
/// Implementação Dapper de <see cref="IClienteRepository"/>. Todo o SQL do
/// módulo Cliente mora aqui; regra de negócio fica no <c>ClienteService</c>.
/// </summary>
public sealed class ClienteRepository : IClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClienteRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    // SELECT comum: campos de cliente + LEFT JOIN nas duas tabelas filhas de
    // especialização e na tabela de endereço. Só uma das filhas PF/PJ traz
    // dados por linha; o endereço vem todo NULL quando o cliente não tem um.
    private const string SelectBase = """
        SELECT
            c.id_cliente        AS IdCliente,
            c.tipo_cliente      AS TipoCliente,
            c.nome              AS Nome,
            c.telefone          AS Telefone,
            c.email             AS Email,
            e.id_endereco       AS IdEndereco,
            e.rua               AS Rua,
            e.numero            AS Numero,
            e.bairro            AS Bairro,
            e.cidade            AS Cidade,
            e.cep               AS Cep,
            c.ativo             AS Ativo,
            pf.cpf              AS Cpf,
            pf.rg               AS Rg,
            pj.cnpj             AS Cnpj,
            pj.nome_fantasia    AS NomeFantasia,
            pj.nome_responsavel AS NomeResponsavel,
            pj.inscricao_estadual AS InscricaoEstadual
        FROM cliente c
        LEFT JOIN cliente_pf pf ON pf.id_cliente = c.id_cliente
        LEFT JOIN cliente_pj pj ON pj.id_cliente = c.id_cliente
        LEFT JOIN endereco   e  ON e.id_endereco = c.id_endereco
        """;

    /// <summary>Linha achatada do SELECT — convertida em <see cref="Cliente"/> por <see cref="Mapear"/>.</summary>
    private sealed class ClienteRow
    {
        public int IdCliente { get; set; }
        public string TipoCliente { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public int? IdEndereco { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Cep { get; set; }
        public bool Ativo { get; set; }
        public string? Cpf { get; set; }
        public string? Rg { get; set; }
        public string? Cnpj { get; set; }
        public string? NomeFantasia { get; set; }
        public string? NomeResponsavel { get; set; }
        public string? InscricaoEstadual { get; set; }
    }

    private static Cliente Mapear(ClienteRow r)
    {
        _ = TipoClienteExtensions.TentarConverter(r.TipoCliente, out var tipo);
        var cliente = new Cliente
        {
            IdCliente = r.IdCliente,
            Tipo = tipo,
            Nome = r.Nome,
            Telefone = r.Telefone,
            Email = r.Email,
            Ativo = r.Ativo,
        };

        if (r.IdEndereco is not null)
        {
            cliente.Endereco = new Endereco
            {
                IdEndereco = r.IdEndereco.Value,
                Rua = r.Rua,
                Numero = r.Numero,
                Bairro = r.Bairro,
                Cidade = r.Cidade,
                Cep = r.Cep,
            };
        }

        if (tipo == TipoCliente.Pf)
        {
            cliente.Pf = new ClientePf { IdCliente = r.IdCliente, Cpf = r.Cpf, Rg = r.Rg };
        }
        else
        {
            cliente.Pj = new ClientePj
            {
                IdCliente = r.IdCliente,
                Cnpj = r.Cnpj,
                NomeFantasia = r.NomeFantasia,
                NomeResponsavel = r.NomeResponsavel,
                InscricaoEstadual = r.InscricaoEstadual,
            };
        }

        return cliente;
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(bool? ativo, string? busca, CancellationToken cancellationToken = default)
    {
        var filtros = new List<string>();
        var parametros = new DynamicParameters();

        if (ativo is not null)
        {
            filtros.Add("c.ativo = @ativo");
            parametros.Add("ativo", ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim();
            parametros.Add("buscaNome", $"%{termo}%");
            var digitos = new string(termo.Where(char.IsDigit).ToArray());

            if (digitos.Length > 0)
            {
                parametros.Add("buscaDoc", $"%{digitos}%");
                filtros.Add("(c.nome LIKE @buscaNome OR pf.cpf LIKE @buscaDoc OR pj.cnpj LIKE @buscaDoc)");
            }
            else
            {
                filtros.Add("c.nome LIKE @buscaNome");
            }
        }

        var where = filtros.Count > 0 ? " WHERE " + string.Join(" AND ", filtros) : string.Empty;
        var sql = SelectBase + where + " ORDER BY c.nome";

        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        var linhas = await conexao.QueryAsync<ClienteRow>(new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
        return linhas.Select(Mapear).ToList();
    }

    public async Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = SelectBase + " WHERE c.id_cliente = @id";
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        var linha = await conexao.QuerySingleOrDefaultAsync<ClienteRow>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        return linha is null ? null : Mapear(linha);
    }

    public async Task<int> InserirAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        await using var transacao = await conexao.BeginTransactionAsync(cancellationToken);

        // Endereço primeiro (se houver): a linha de cliente precisa do id_endereco.
        int? idEndereco = null;
        if (cliente.Endereco is { Vazio: false } endereco)
        {
            idEndereco = await InserirEnderecoAsync(conexao, transacao, endereco, cancellationToken);
        }

        const string inserirCliente = """
            INSERT INTO cliente (tipo_cliente, nome, telefone, email, id_endereco, ativo)
            VALUES (@TipoCliente, @Nome, @Telefone, @Email, @IdEndereco, @Ativo);
            SELECT LAST_INSERT_ID();
            """;

        var novoId = (int)await conexao.ExecuteScalarAsync<ulong>(new CommandDefinition(
            inserirCliente,
            new
            {
                TipoCliente = cliente.Tipo.ParaTexto(),
                cliente.Nome,
                cliente.Telefone,
                cliente.Email,
                IdEndereco = idEndereco,
                cliente.Ativo,
            },
            transacao,
            cancellationToken: cancellationToken));

        if (cliente.Tipo == TipoCliente.Pf)
        {
            const string inserirPf = "INSERT INTO cliente_pf (id_cliente, cpf, rg) VALUES (@Id, @Cpf, @Rg);";
            await conexao.ExecuteAsync(new CommandDefinition(
                inserirPf,
                new { Id = novoId, cliente.Pf!.Cpf, cliente.Pf!.Rg },
                transacao,
                cancellationToken: cancellationToken));
        }
        else
        {
            const string inserirPj = """
                INSERT INTO cliente_pj (id_cliente, cnpj, nome_fantasia, nome_responsavel, inscricao_estadual)
                VALUES (@Id, @Cnpj, @NomeFantasia, @NomeResponsavel, @InscricaoEstadual);
                """;
            await conexao.ExecuteAsync(new CommandDefinition(
                inserirPj,
                new
                {
                    Id = novoId,
                    cliente.Pj!.Cnpj,
                    cliente.Pj!.NomeFantasia,
                    cliente.Pj!.NomeResponsavel,
                    cliente.Pj!.InscricaoEstadual,
                },
                transacao,
                cancellationToken: cancellationToken));
        }

        await transacao.CommitAsync(cancellationToken);
        return novoId;
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        await using var transacao = await conexao.BeginTransactionAsync(cancellationToken);

        // id_endereco atual (dentro da transação) para decidir INSERT/UPDATE/remoção.
        var idEnderecoAtual = await conexao.ExecuteScalarAsync<uint?>(new CommandDefinition(
            "SELECT id_endereco FROM cliente WHERE id_cliente = @IdCliente;",
            new { cliente.IdCliente },
            transacao,
            cancellationToken: cancellationToken));

        int? idEnderecoFinal = idEnderecoAtual is null ? null : (int)idEnderecoAtual.Value;

        if (cliente.Endereco is { Vazio: false } endereco)
        {
            if (idEnderecoAtual is null)
            {
                idEnderecoFinal = await InserirEnderecoAsync(conexao, transacao, endereco, cancellationToken);
            }
            else
            {
                const string atualizarEndereco = """
                    UPDATE endereco
                       SET rua = @Rua, numero = @Numero, bairro = @Bairro, cidade = @Cidade, cep = @Cep
                     WHERE id_endereco = @IdEndereco;
                    """;
                await conexao.ExecuteAsync(new CommandDefinition(
                    atualizarEndereco,
                    new { endereco.Rua, endereco.Numero, endereco.Bairro, endereco.Cidade, endereco.Cep, IdEndereco = idEnderecoFinal },
                    transacao,
                    cancellationToken: cancellationToken));
            }
        }
        else if (idEnderecoAtual is not null)
        {
            // Endereço foi esvaziado: desfaz a FK e apaga a linha órfã. DELETE físico
            // aqui é de um value-object sem histórico próprio (não é um cadastro) —
            // aceitável, ao contrário de DELETE de cliente (E01 RN06).
            idEnderecoFinal = null;
        }

        const string atualizarCliente = """
            UPDATE cliente
               SET nome = @Nome, telefone = @Telefone, email = @Email, id_endereco = @IdEndereco
             WHERE id_cliente = @IdCliente;
            """;
        await conexao.ExecuteAsync(new CommandDefinition(
            atualizarCliente,
            new { cliente.Nome, cliente.Telefone, cliente.Email, IdEndereco = idEnderecoFinal, cliente.IdCliente },
            transacao,
            cancellationToken: cancellationToken));

        if (idEnderecoAtual is not null && idEnderecoFinal is null)
        {
            await conexao.ExecuteAsync(new CommandDefinition(
                "DELETE FROM endereco WHERE id_endereco = @Id;",
                new { Id = (int)idEnderecoAtual.Value },
                transacao,
                cancellationToken: cancellationToken));
        }

        if (cliente.Tipo == TipoCliente.Pf)
        {
            const string atualizarPf = "UPDATE cliente_pf SET cpf = @Cpf, rg = @Rg WHERE id_cliente = @IdCliente;";
            await conexao.ExecuteAsync(new CommandDefinition(
                atualizarPf,
                new { cliente.Pf!.Cpf, cliente.Pf!.Rg, cliente.IdCliente },
                transacao,
                cancellationToken: cancellationToken));
        }
        else
        {
            const string atualizarPj = """
                UPDATE cliente_pj
                   SET cnpj = @Cnpj, nome_fantasia = @NomeFantasia,
                       nome_responsavel = @NomeResponsavel, inscricao_estadual = @InscricaoEstadual
                 WHERE id_cliente = @IdCliente;
                """;
            await conexao.ExecuteAsync(new CommandDefinition(
                atualizarPj,
                new
                {
                    cliente.Pj!.Cnpj,
                    cliente.Pj!.NomeFantasia,
                    cliente.Pj!.NomeResponsavel,
                    cliente.Pj!.InscricaoEstadual,
                    cliente.IdCliente,
                },
                transacao,
                cancellationToken: cancellationToken));
        }

        await transacao.CommitAsync(cancellationToken);
    }

    public async Task<bool> DefinirAtivoAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        // E01 RN06/RN07 — só alterna a flag; nunca DELETE de cliente, nunca mexe
        // nas filhas nem no endereço (E01 RN08 — histórico intacto).
        const string sql = "UPDATE cliente SET ativo = @ativo WHERE id_cliente = @id;";
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        var afetadas = await conexao.ExecuteAsync(new CommandDefinition(sql, new { id, ativo }, cancellationToken: cancellationToken));
        return afetadas > 0;
    }

    public async Task<bool> ExisteAtivoComCpfAsync(string cpf, int? idExcluir, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
              FROM cliente c
              JOIN cliente_pf pf ON pf.id_cliente = c.id_cliente
             WHERE c.ativo = 1
               AND pf.cpf = @cpf
               AND (@idExcluir IS NULL OR c.id_cliente <> @idExcluir);
            """;
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        var total = await conexao.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, new { cpf, idExcluir }, cancellationToken: cancellationToken));
        return total > 0;
    }

    public async Task<bool> ExisteAtivoComCnpjAsync(string cnpj, int? idExcluir, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
              FROM cliente c
              JOIN cliente_pj pj ON pj.id_cliente = c.id_cliente
             WHERE c.ativo = 1
               AND pj.cnpj = @cnpj
               AND (@idExcluir IS NULL OR c.id_cliente <> @idExcluir);
            """;
        await using var conexao = await _connectionFactory.AbrirAsync(cancellationToken);
        var total = await conexao.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, new { cnpj, idExcluir }, cancellationToken: cancellationToken));
        return total > 0;
    }

    private static async Task<int> InserirEnderecoAsync(
        MySqlConnection conexao,
        MySqlTransaction transacao,
        Endereco endereco,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO endereco (rua, numero, bairro, cidade, cep)
            VALUES (@Rua, @Numero, @Bairro, @Cidade, @Cep);
            SELECT LAST_INSERT_ID();
            """;
        var id = await conexao.ExecuteScalarAsync<ulong>(new CommandDefinition(
            sql,
            new { endereco.Rua, endereco.Numero, endereco.Bairro, endereco.Cidade, endereco.Cep },
            transacao,
            cancellationToken: cancellationToken));
        return (int)id;
    }
}
