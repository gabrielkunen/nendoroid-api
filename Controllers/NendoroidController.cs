using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Request;
using NendoroidApi.Response.Base;
using NendoroidApi.Response.Common;

namespace NendoroidApi.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
public class NendoroidController(INendoroidRepository nendoroidRepository) : ControllerBase
{
    private readonly INendoroidRepository _nendoroidRepository = nendoroidRepository;

    [HttpPost]
    [ProducesResponseType(typeof(ModelValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(CustomResponse<Nendoroid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] NendoroidRequest nendoroidRequest)
    {
        var nendoroidJaExiste = await _nendoroidRepository.Any(nendoroidRequest.Numeracao);

        if(nendoroidJaExiste)
            return Conflict(new ConflictResponse("Nendoroid com esta numeração já cadastrada."));

        Nendoroid nendoroid = nendoroidRequest;

        await _nendoroidRepository.Add(nendoroid);
        
        return CreatedAtAction(nameof(Get), new { numeracao = nendoroid.Numeracao }, 
            new CustomResponse<Nendoroid>(HttpStatusCode.OK, "Nendoroid cadastrada com sucesso!", nendoroid));
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<Nendoroid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Nendoroid>> Get([FromQuery] string numeracao)
    {
        var nendoroid = await _nendoroidRepository.Get(numeracao);

        if(nendoroid == null)
            return NotFound(new NotFoundResponse("Nendoroid com esta numeração não cadastrada."));

        return Ok(new CustomResponse<Nendoroid>(HttpStatusCode.OK, "Nendoroid encontrada com sucesso!", nendoroid));
    }
}
