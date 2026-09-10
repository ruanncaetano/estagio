using Microsoft.AspNetCore.Mvc;

namespace tcc.Controllers;

/// <summary>
/// Endpoint mínimo de verificação — serve de referência viva da camada
/// Controller: só entrada/saída HTTP, nenhuma regra de negócio aqui.
/// Regra de negócio mora em <c>Services/</c>; acesso a dados em <c>Repositories/</c>.
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", service = "fogo-erp-api" });
}
