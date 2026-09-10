using Microsoft.AspNetCore.Mvc;

namespace tcc.Controllers;

/// <summary>
/// Verificação de disponibilidade da API. Serve também de referência viva do
/// padrão de documentação: todo endpoint tem <c>&lt;summary&gt;</c> e
/// <see cref="ProducesResponseTypeAttribute"/> para o Swagger.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>Retorna 200 se a API está no ar.</summary>
    /// <response code="200">Serviço disponível.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "ok", service = "fogo-erp-api" });
}
