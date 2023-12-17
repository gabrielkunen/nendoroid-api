using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace NendoroidApi.V2.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ApiVersion("2.0", Deprecated = true)]
[Route("v{version:apiVersion}/[controller]")]
[EnableRateLimiting("ApiBlock")]
public class TesteController() : ControllerBase
{

    /// <summary>
    /// V2 Version
    /// <returns>Ok</returns>
    /// <response code="200">Ok</response>
    /// <response code="500">Erro interno</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}
