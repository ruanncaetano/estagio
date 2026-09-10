using Microsoft.AspNetCore.Mvc;
using tcc.Common;
using tcc.Models.Dtos;
using tcc.Services;

namespace tcc.Controllers;

/// <summary>
/// CRUD de clientes (Pessoa Física e Jurídica) — Estória 01. "Remover" um
/// cliente é inativá-lo (<c>PATCH /clientes/{id}/inativar</c>), nunca apagar.
/// </summary>
[ApiController]
[Route("clientes")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
        => _clienteService = clienteService;

    /// <summary>Lista clientes, com filtro opcional por situação e busca por nome ou documento.</summary>
    /// <param name="ativo">
    /// <c>true</c> = só ativos; <c>false</c> = só inativos; omitido = todos.
    /// </param>
    /// <param name="busca">Trecho do nome/razão social ou do CPF/CNPJ (só dígitos).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Lista (possivelmente vazia) de clientes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteResponse>>> Listar(
        [FromQuery] bool? ativo,
        [FromQuery] string? busca,
        CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ListarAsync(ativo, busca, cancellationToken);
        return Ok(clientes);
    }

    /// <summary>Obtém um cliente pelo identificador.</summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Não existe cliente com esse id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var resultado = await _clienteService.ObterPorIdAsync(id, cancellationToken);
        return Traduzir(resultado);
    }

    /// <summary>Cadastra um cliente PF ou PJ.</summary>
    /// <param name="request">Dados do cliente. <c>tipo</c> = "PF" ou "PJ" (E01 RN02).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="201">Cliente criado; corpo com o recurso e header <c>Location</c>.</response>
    /// <response code="400">
    /// Entrada mal formada (DataAnnotations do DTO) ou regra de formato do
    /// Service — tipo, nome ou documento inválido (E01 RN02/RN03/RN04).
    /// </response>
    /// <response code="409">Já existe um cliente ativo com o mesmo CPF/CNPJ (E01 RN05).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Criar(
        [FromBody] CriarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _clienteService.CriarAsync(request, cancellationToken);
        if (!resultado.Sucesso)
        {
            return TraduzirFalha(resultado);
        }

        var criado = resultado.Valor!;
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdCliente }, criado);
    }

    /// <summary>Edita os dados de um cliente existente (o tipo PF/PJ não muda).</summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="request">Novos dados do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente atualizado.</response>
    /// <response code="400">
    /// Entrada mal formada (DataAnnotations do DTO) ou regra de formato do
    /// Service — nome ou documento inválido (E01 RN03/RN04).
    /// </response>
    /// <response code="404">Não existe cliente com esse id.</response>
    /// <response code="409">Já existe outro cliente ativo com o mesmo CPF/CNPJ (E01 RN05).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Atualizar(
        int id,
        [FromBody] AtualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _clienteService.AtualizarAsync(id, request, cancellationToken);
        return Traduzir(resultado);
    }

    /// <summary>Inativa um cliente (E01 RN06/RN07). Idempotente. Preserva histórico e vínculos (E01 RN08).</summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente inativado (ou já estava inativo).</response>
    /// <response code="404">Não existe cliente com esse id.</response>
    [HttpPatch("{id:int}/inativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        var resultado = await _clienteService.InativarAsync(id, cancellationToken);
        return resultado.Sucesso ? Ok() : TraduzirFalha(resultado);
    }

    /// <summary>Reativa um cliente inativo. Revalida a unicidade de CPF/CNPJ entre ativos (E01 RN05).</summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <response code="200">Cliente reativado (ou já estava ativo).</response>
    /// <response code="404">Não existe cliente com esse id.</response>
    /// <response code="409">Há outro cliente ativo com o mesmo CPF/CNPJ (E01 RN05).</response>
    [HttpPatch("{id:int}/reativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reativar(int id, CancellationToken cancellationToken)
    {
        var resultado = await _clienteService.ReativarAsync(id, cancellationToken);
        return resultado.Sucesso ? Ok() : TraduzirFalha(resultado);
    }

    // --- Tradução Result -> HTTP ---

    private ActionResult<ClienteResponse> Traduzir(Result<ClienteResponse> resultado)
        => resultado.Sucesso ? Ok(resultado.Valor) : TraduzirFalha(resultado);

    /// <summary>
    /// Converte uma falha de RN no status HTTP correspondente:
    /// Validação → 400, NãoEncontrado → 404, Conflito → 409.
    /// </summary>
    private ObjectResult TraduzirFalha(Result resultado)
    {
        var (status, titulo) = resultado.Tipo switch
        {
            TipoFalha.Validacao => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            TipoFalha.NaoEncontrado => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            TipoFalha.Conflito => (StatusCodes.Status409Conflict, "Conflito"),
            _ => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        };

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = resultado.Mensagem,
        };
        if (resultado.Codigo is not null)
        {
            problema.Extensions["codigo"] = resultado.Codigo;
        }

        return StatusCode(status, problema);
    }
}
