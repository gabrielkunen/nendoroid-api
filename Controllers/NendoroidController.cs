using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Request;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ErroResponse), StatusCodes.Status500InternalServerError)]
[Route("[controller]")]
public class NendoroidController(INendoroidRepository nendoroidRepository) : ControllerBase
{
    private readonly INendoroidRepository _nendoroidRepository = nendoroidRepository;

    [HttpPost]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Nendoroid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] NendoroidRequest nendoroidRequest)
    {
        var nendoroidJaExiste = await _nendoroidRepository.Any(nendoroidRequest.Numeracao);

        if(nendoroidJaExiste)
            return BadRequest(new ErroResponse(StatusCodes.Status400BadRequest, "Nendoroid com esta numeração já cadastrada."));

        Nendoroid nendoroid = nendoroidRequest;

        await _nendoroidRepository.Add(nendoroid);
        
        return CreatedAtAction(nameof(Get), new { numeracao = nendoroid.Numeracao }, nendoroid);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ActionResult<Nendoroid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Nendoroid>> Get([FromQuery] string numeracao)
    {
        var nendoroid = await _nendoroidRepository.Get(numeracao);

        if(nendoroid == null)
            return NotFound(new ErroResponse(StatusCodes.Status404NotFound, "Nendoroid com esta numeração não cadastrada."));

        return Ok(nendoroid);
    }
}
