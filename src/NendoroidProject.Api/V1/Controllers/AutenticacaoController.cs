﻿using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NendoroidProject.Api.Request;
using NendoroidProject.Api.Response;
using NendoroidProject.Api.Response.Base;
using NendoroidProject.Api.Response.Common;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Interfaces.Services;

namespace NendoroidProject.Api.V1.Controllers;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/autenticacao")]
[EnableRateLimiting("ApiBlock")]
public class AutenticacaoController(IUsuarioRepository usuarioRepository, ITokenService tokenService) : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly ITokenService _tokenService = tokenService;

    /// <summary>
    /// Login do usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Token de autenticação JWT</returns>
    /// <remarks>
    /// Request exemplo:
    ///
    ///     POST /Login
    ///     {
    ///        "nome": "Gabriel",
    ///        "senha": "senha.teste"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="400">Erro no corpo da requisição</response>
    /// <response code="401">Usuário ou senha inválidos.</response>
    /// <response code="429">Excesso de requisições.</response>
    /// <response code="500">Erro interno</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ModelValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NaoAutenticadoResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(TooManyRequestResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var usuario = await _usuarioRepository.Get(request.Nome);

        if(usuario == null)
            return Unauthorized(new NaoAutenticadoResponse("Usuário ou senha inválidos."));

        var result = _tokenService.SenhaValida(request.Senha, usuario.Senha);

        if(result) {
            var token = _tokenService.Gerar(usuario);
            return Ok(new CustomResponse<LoginResponse>(HttpStatusCode.OK, "Logado com sucesso.", new LoginResponse(request.Nome, token)));
        }
        else
            return Unauthorized(new NaoAutenticadoResponse("Usuário ou senha inválidos."));
    }
}
