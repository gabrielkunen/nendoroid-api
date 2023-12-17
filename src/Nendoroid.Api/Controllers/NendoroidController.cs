using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Domain.Repositories.Base;
using NendoroidApi.Filters;
using NendoroidApi.Request;
using NendoroidApi.Response.Base;
using NendoroidApi.Response.Common;

namespace NendoroidApi.Controllers;

/// <summary>
/// Nendoroid Controller
/// </summary>
/// <param name="unitOfWork"></param>
/// <param name="nendoroidRepository"></param>
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
[EnableRateLimiting("ApiBlock")]
public class NendoroidController(IUnitOfWork unitOfWork, INendoroidRepository nendoroidRepository) : ControllerBase
{
    private readonly INendoroidRepository _nendoroidRepository = nendoroidRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Cadastro de uma nova nendoroid
    /// </summary>
    /// <param name="nendoroidRequest"></param>
    /// <returns>A nova nendoroid criada</returns>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     POST /Nendoroid
    ///     {
    ///        "nome": "Nendoroid Komari Koshigaya",
    ///        "numeracao": "1583",
    ///        "preco": 5000,
    ///        "serie": "Non Non Biyori Nonstop",
    ///        "fabricante": "Good Smile Company",
    ///        "escultor": "Design COCO",
    ///        "cooperacao": "Nendoron",
    ///        "datalancamento": "2021-09",
    ///        "url": "https://www.goodsmile.info/en/product/10894/Nendoroid+Komari+Koshigaya.html",
    ///        "especificacoes": "Painted ABS&amp;PVC non-scale articulated figure with stand included. Approximately 100mm in height.",
    ///        "imagens": [
    ///            "https://images.goodsmile.info/cgm/images/product/20210303/10894/81844/large/06c15c220bd14006d387540da50a45bd.jpg", 
    ///            "https://images.goodsmile.info/cgm/images/product/20210303/10894/81845/large/59d5c7c0bc7f62ab56890f758a1e52b0.jpg"
    ///        ]
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Nendoroid cadastrada com sucesso</response>
    /// <response code="400">Erro no corpo da requisição</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não autorizado</response>
    /// <response code="409">Nendoroid já cadastrada</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpPost]
    [CustomAuthorize(Roles = ["admin"])]
    [ProducesResponseType(typeof(CustomResponse<Nendoroid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ModelValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NaoAutorizadoResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] NendoroidRequest nendoroidRequest)
    {
        var nendoroidJaExiste = await _nendoroidRepository.Any(nendoroidRequest.Numeracao);

        if(nendoroidJaExiste)
            return Conflict(new ConflictResponse("Nendoroid com esta numeração já cadastrada."));

        Nendoroid nendoroid = nendoroidRequest;
        nendoroid.SetarDataCadastroUtcNow();
        var listaNendoroidImagens = new List<NendoroidImagens>();

        _unitOfWork.BeginTransaction();

        var idNendoroid = await _nendoroidRepository.Add(nendoroid);

        for (var i = 0; i < nendoroidRequest.Imagens.Length; i++)
            listaNendoroidImagens.Add(new NendoroidImagens(idNendoroid, nendoroidRequest.Imagens[i]));

        await _nendoroidRepository.AddImagens(listaNendoroidImagens);

        _unitOfWork.Commit();

        return CreatedAtAction(nameof(Get), new { numeracao = nendoroid.Numeracao }, 
            new CustomResponse<Nendoroid>(HttpStatusCode.Created, "Nendoroid cadastrada com sucesso.", nendoroid));
    }

    /// <summary>
    /// Buscar dados de uma nendoroid
    /// </summary>
    /// <param name="numeracao"></param>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     GET /Nendoroid?numeracao=1583
    ///
    /// </remarks>
    /// <returns>Dados da nendoroid buscada</returns>
    /// <response code="200">Nendoroid encontrada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não autorizado</response>
    /// <response code="404">Nendoroid não encontrada</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpGet]
    [CustomAuthorize(Roles = ["comum", "admin"])]
    [ProducesResponseType(typeof(CustomResponse<Nendoroid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NaoAutorizadoResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Nendoroid>> Get([FromQuery] string numeracao)
    {
        var nendoroid = await _nendoroidRepository.Get(numeracao);

        if(nendoroid == null)
            return NotFound(new NotFoundResponse("Nendoroid com esta numeração não cadastrada."));

        return Ok(new CustomResponse<Nendoroid>(HttpStatusCode.OK, "Nendoroid encontrada com sucesso.", nendoroid));
    }

    /// <summary>
    /// Deletar uma nendoroid
    /// </summary>
    /// <param name="numeracao"></param>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     DELETE /Nendoroid?numeracao=1583
    ///
    /// </remarks>
    /// <returns>Mensagem padrão</returns>
    /// <response code="200">Nendoroid deletada com sucesso</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não autorizado</response>
    /// <response code="404">Nendoroid não encontrada</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpDelete]
    [CustomAuthorize(Roles = ["admin"])]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NaoAutorizadoResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromQuery] string numeracao)
    {
        var nendoroidExiste = await _nendoroidRepository.Any(numeracao);

        if(!nendoroidExiste)
            return NotFound(new NotFoundResponse("Nendoroid com esta numeração não cadastrada."));

        await _nendoroidRepository.Delete(numeracao);

        return Ok(new OkResponse("Nendoroid deletada com sucesso."));
    }

    /// <summary>
    /// Editar os dados de uma nendoroid
    /// </summary>
    /// <param name="nendoroidRequest"></param>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     PUT /Nendoroid
    ///     {
    ///        "nome": "Nendoroid Komari Koshigaya",
    ///        "numeracao": "1583",
    ///        "preco": 5000,
    ///        "serie": "Non Non Biyori Nonstop",
    ///        "fabricante": "Good Smile Company",
    ///        "escultor": "Design COCO",
    ///        "cooperacao": "Nendoron",
    ///        "datalancamento": "2021-09",
    ///        "url": "https://www.goodsmile.info/en/product/10894/Nendoroid+Komari+Koshigaya.html",
    ///        "especificacoes": "Painted ABS&amp;PVC non-scale articulated figure with stand included. Approximately 100mm in height.",
    ///        "imagens": [
    ///            "https://images.goodsmile.info/cgm/images/product/20210303/10894/81844/large/06c15c220bd14006d387540da50a45bd.jpg", 
    ///            "https://images.goodsmile.info/cgm/images/product/20210303/10894/81845/large/59d5c7c0bc7f62ab56890f758a1e52b0.jpg"
    ///        ]
    ///     }
    ///
    /// </remarks>
    /// <returns>Mensagem padrão</returns>
    /// <response code="200">Nendoroid alterada com sucesso</response>
    /// <response code="400">Erro no corpo da requisição</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não autorizado</response>
    /// <response code="404">Nendoroid não encontrada</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpPut]
    [CustomAuthorize(Roles = ["admin"])]
    [ProducesResponseType(typeof(OkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ModelValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NaoAutorizadoResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put([FromBody] NendoroidRequest nendoroidRequest)
    {
        var nendoroidExiste = await _nendoroidRepository.Any(nendoroidRequest.Numeracao);

        if(!nendoroidExiste)
            return NotFound(new NotFoundResponse("Nendoroid com esta numeração não cadastrada."));

        Nendoroid nendoroid = nendoroidRequest;
        nendoroid.SetarDataAlteracaoUtcNow();
        var listaNendoroidImagens = new List<NendoroidImagens>();

        _unitOfWork.BeginTransaction();

        var idNendoroid = await _nendoroidRepository.Update(nendoroid);

        for (var i = 0; i < nendoroidRequest.Imagens.Length; i++)
            listaNendoroidImagens.Add(new NendoroidImagens(idNendoroid, nendoroidRequest.Imagens[i]));

        await _nendoroidRepository.DeleteImagens(idNendoroid);
        await _nendoroidRepository.AddImagens(listaNendoroidImagens);

        _unitOfWork.Commit();

        return Ok(new OkResponse("Nendoroid atualizada com sucesso."));
    }
}
