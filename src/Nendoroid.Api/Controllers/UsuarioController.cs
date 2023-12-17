using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Domain.Repositories.Base;
using NendoroidApi.Filters;
using NendoroidApi.Response.Base;
using NendoroidApi.Response.Common;

namespace NendoroidApi.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
[EnableRateLimiting("ApiBlock")]
public class UsuarioController(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Cadastro de um novo usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Mensagem padrão</returns>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     POST /Usuario
    ///     {
    ///        "nome": "Gabriel",
    ///        "senha": "senha.teste"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Erro no corpo da requisição</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Não autorizado</response>
    /// <response code="409">Usuário já cadastrado</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpPost]
    [CustomAuthorize(Roles = ["admin"])]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ModelValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(NaoAutorizadoResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(CriarUsuarioRequest request)
    {
        var usuarioJaExiste = await _usuarioRepository.Any(request.Nome);

        if(usuarioJaExiste)
            return Conflict(new ConflictResponse("Usuário com este nome já cadastrado."));

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha, workFactor: 14);

        Usuario usuario = request;
        usuario.SetarDataCadastroUtcNow();
        usuario.SetarSenhaHashUsuario(passwordHash);
        usuario.SetarCargoUsuario("comum");

        _unitOfWork.BeginTransaction();

        await _usuarioRepository.Add(usuario);

        _unitOfWork.Commit();

        return CreatedAtAction(nameof(Post), new { nome = usuario.Nome }, 
            new CustomResponse<object>(HttpStatusCode.Created, "Usuário cadastrado com sucesso."));
    }
}
