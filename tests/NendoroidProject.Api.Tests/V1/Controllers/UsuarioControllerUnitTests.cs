using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using NendoroidProject.Api.Request;
using NendoroidProject.Api.Response.Common;
using NendoroidProject.Api.V1.Controllers;
using NendoroidProject.Domain.Interfaces.Repository;
using Xunit;

namespace NendoroidProject.Api.Tests.V1.Controllers
{
    public class UsuarioControllerUnitTests
    {
        private readonly AutoMocker _mocker;
        private readonly UsuarioController _controller;
        public UsuarioControllerUnitTests()
        {
            _mocker = new AutoMocker();
            _controller = _mocker.CreateInstance<UsuarioController>();
        }

        [Fact]
        public async Task Post_DeveRetornarUsuarioJaCadastrado()
        {
            var request = new CriarUsuarioRequest();

            _mocker.GetMock<IUsuarioRepository>().Setup(r => r.Any(It.IsAny<string>())).ReturnsAsync(true);

            var retorno = await _controller.Post(request) as ObjectResult;
            var responseBody = retorno?.Value as ConflictResponse;

            Assert.Equal(StatusCodes.Status409Conflict, retorno?.StatusCode);
            Assert.Equal("Usuário com este nome já cadastrado.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Post_DeveCadastrarUsuario()
        {
            var request = new CriarUsuarioRequest();

            _mocker.GetMock<IUsuarioRepository>().Setup(r => r.Any(It.IsAny<string>())).ReturnsAsync(false);

            var retorno = await _controller.Post(request) as ObjectResult;
            var responseBody = retorno?.Value as CustomResponse<object>;

            Assert.Equal(StatusCodes.Status201Created, retorno?.StatusCode);
            Assert.Equal("Usuário cadastrado com sucesso.", responseBody?.Mensagem);
        }
    }
}
