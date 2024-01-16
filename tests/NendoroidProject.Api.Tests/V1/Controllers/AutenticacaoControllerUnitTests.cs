using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using NendoroidProject.Api.Request;
using NendoroidProject.Api.Response;
using NendoroidProject.Api.Response.Common;
using NendoroidProject.Api.V1.Controllers;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Interfaces.Services;
using NendoroidProject.Domain.Models;
using Xunit;

namespace NendoroidProject.Api.Tests.V1.Controllers
{
    public class AutenticacaoControllerUnitTests
    {
        private readonly AutoMocker _mocker;
        private readonly AutenticacaoController _controller;
        public AutenticacaoControllerUnitTests()
        {
            _mocker = new AutoMocker();
            _controller = _mocker.CreateInstance<AutenticacaoController>();
        }

        [Fact]
        public async Task Login_DeveRetornarUsuarioOuSenhaIncorretos()
        {
            var request = new LoginRequest();

            _mocker.GetMock<IUsuarioRepository>().Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync((Usuario?) null);

            var retorno = await _controller.Login(request) as ObjectResult;
            var responseBody = retorno?.Value as NaoAutenticadoResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, retorno?.StatusCode);
            Assert.Equal("Usuário ou senha inválidos.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Login_DeveRetornarSenhaIncorreta()
        {
            var request = new LoginRequest();

            _mocker.GetMock<IUsuarioRepository>().Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(new Usuario());
            _mocker.GetMock<ITokenService>().Setup(r => r.SenhaValida(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var retorno = await _controller.Login(request) as ObjectResult;
            var responseBody = retorno?.Value as NaoAutenticadoResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, retorno?.StatusCode);
            Assert.Equal("Usuário ou senha inválidos.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Login_DeveRetornarLoginValido()
        {
            var request = new LoginRequest();

            _mocker.GetMock<IUsuarioRepository>().Setup(r => r.Get(It.IsAny<string>())).ReturnsAsync(new Usuario());
            _mocker.GetMock<ITokenService>().Setup(r => r.SenhaValida(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var retorno = await _controller.Login(request) as ObjectResult;
            var responseBody = retorno?.Value as CustomResponse<LoginResponse>;

            Assert.Equal(StatusCodes.Status200OK, retorno?.StatusCode);
            Assert.Equal("Logado com sucesso.", responseBody?.Mensagem);
        }
    }
}
