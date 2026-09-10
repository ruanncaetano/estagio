namespace tcc.Common;

/// <summary>
/// Classifica uma falha de regra de negócio para o Controller escolher o
/// status HTTP. Falha de RN <b>não</b> é exceção — o Service devolve um
/// <see cref="Result"/>/<see cref="Result{T}"/> e o Controller traduz.
/// </summary>
public enum TipoFalha
{
    /// <summary>Nenhuma falha (resultado de sucesso).</summary>
    Nenhuma = 0,

    /// <summary>Dado inválido / regra de formato. → HTTP 400.</summary>
    Validacao = 1,

    /// <summary>Registro não encontrado. → HTTP 404.</summary>
    NaoEncontrado = 2,

    /// <summary>Conflito com o estado atual (ex.: E01 RN05, documento duplicado). → HTTP 409.</summary>
    Conflito = 3,
}

/// <summary>
/// Resultado de uma operação de Service sem valor de retorno: sucesso, ou
/// falha com <see cref="Tipo"/> + <see cref="Mensagem"/> + <see cref="Codigo"/>
/// (identificador curto e estável, ex.: "E01_RN05").
/// </summary>
public class Result
{
    /// <summary>Operação concluída sem violar nenhuma RN.</summary>
    public bool Sucesso { get; }

    /// <summary>Classificação da falha (irrelevante quando <see cref="Sucesso"/>).</summary>
    public TipoFalha Tipo { get; }

    /// <summary>Mensagem pronta para exibir ao usuário.</summary>
    public string? Mensagem { get; }

    /// <summary>Código curto/estável da falha para o cliente tratar (ex.: "E01_RN05").</summary>
    public string? Codigo { get; }

    private protected Result(bool sucesso, TipoFalha tipo, string? mensagem, string? codigo)
    {
        Sucesso = sucesso;
        Tipo = tipo;
        Mensagem = mensagem;
        Codigo = codigo;
    }

    /// <summary>Cria um resultado de sucesso.</summary>
    public static Result Ok() => new(true, TipoFalha.Nenhuma, null, null);

    /// <summary>Cria uma falha de validação (→ 400).</summary>
    public static Result Validacao(string mensagem, string? codigo = null)
        => new(false, TipoFalha.Validacao, mensagem, codigo);

    /// <summary>Cria uma falha "não encontrado" (→ 404).</summary>
    public static Result NaoEncontrado(string mensagem, string? codigo = null)
        => new(false, TipoFalha.NaoEncontrado, mensagem, codigo);

    /// <summary>Cria uma falha de conflito (→ 409).</summary>
    public static Result Conflito(string mensagem, string? codigo = null)
        => new(false, TipoFalha.Conflito, mensagem, codigo);
}

/// <summary>
/// Como <see cref="Result"/>, mas carrega um <typeparamref name="T"/> no
/// caso de sucesso (ex.: o <c>ClienteResponse</c> recém-criado).
/// </summary>
public sealed class Result<T> : Result
{
    /// <summary>Valor produzido — só é significativo quando <see cref="Result.Sucesso"/>.</summary>
    public T? Valor { get; }

    private Result(bool sucesso, TipoFalha tipo, string? mensagem, string? codigo, T? valor)
        : base(sucesso, tipo, mensagem, codigo)
        => Valor = valor;

    /// <summary>Cria um resultado de sucesso com valor.</summary>
    public static Result<T> Ok(T valor) => new(true, TipoFalha.Nenhuma, null, null, valor);

    /// <summary>Cria uma falha de validação (→ 400).</summary>
    public static new Result<T> Validacao(string mensagem, string? codigo = null)
        => new(false, TipoFalha.Validacao, mensagem, codigo, default);

    /// <summary>Cria uma falha "não encontrado" (→ 404).</summary>
    public static new Result<T> NaoEncontrado(string mensagem, string? codigo = null)
        => new(false, TipoFalha.NaoEncontrado, mensagem, codigo, default);

    /// <summary>Cria uma falha de conflito (→ 409).</summary>
    public static new Result<T> Conflito(string mensagem, string? codigo = null)
        => new(false, TipoFalha.Conflito, mensagem, codigo, default);
}
