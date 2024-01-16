using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using NendoroidProject.Api.Request;
using NendoroidProject.Api.Response.Common;
using NendoroidProject.Api.V1.Controllers;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Models;
using Xunit;

namespace NendoroidProject.Api.Tests.V1.Controllers
{
    public class NendoroidControllerUnitTests
    {
        private readonly AutoMocker _mocker;
        private readonly NendoroidController _controller;
        public NendoroidControllerUnitTests()
        {
            _mocker = new AutoMocker();
            _controller = _mocker.CreateInstance<NendoroidController>();
        }

        [Fact]
        public async Task Post_DeveRetornarErroNendoroidJaCadastrada()
        {
            var request = new NendoroidRequest();

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(true);

            var retorno = await _controller.Post(request) as ObjectResult;
            var responseBody = retorno?.Value as ConflictResponse;

            Assert.Equal(StatusCodes.Status409Conflict, retorno?.StatusCode);
            Assert.Equal("Nendoroid com esta numeração já cadastrada.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Post_DeveCadastrarNendoroid()
        {
            var request = new NendoroidRequest
            {
                Imagens =
                [
                    "https://google.com.br/1", "https://google.com.br/2", "https://google.com.br/3"
                ]
            };

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(false);
            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Add(It.IsAny<Nendoroid>())).ReturnsAsync(1);

            var retorno = await _controller.Post(request) as ObjectResult;
            var responseBody = retorno?.Value as CustomResponse<Nendoroid>;

            Assert.Equal(StatusCodes.Status201Created, retorno?.StatusCode);
            Assert.Equal("Nendoroid cadastrada com sucesso.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Get_DeveRetornarNendoroidNaoCadastrada()
        {
            var request = "1583";

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Get(It.IsAny<string>())).ReturnsAsync((Nendoroid?) null);

            var retorno = await _controller.Get(request) as ObjectResult;
            var responseBody = retorno?.Value as NotFoundResponse;

            Assert.Equal(StatusCodes.Status404NotFound, retorno?.StatusCode);
            Assert.Equal("Nendoroid com esta numeração não cadastrada.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Get_DeveRetornarNendoroid()
        {
            var request = "1583";

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Get(It.IsAny<string>())).ReturnsAsync(new Nendoroid());

            var retorno = await _controller.Get(request) as ObjectResult;
            var responseBody = retorno?.Value as CustomResponse<Nendoroid>;

            Assert.Equal(StatusCodes.Status200OK, retorno?.StatusCode);
            Assert.Equal("Nendoroid encontrada com sucesso.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Delete_DeveRetornarNendoroidNaoCadastrada()
        {
            var request = "1583";

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(false);

            var retorno = await _controller.Delete(request) as ObjectResult;
            var responseBody = retorno?.Value as NotFoundResponse;

            Assert.Equal(StatusCodes.Status404NotFound, retorno?.StatusCode);
            Assert.Equal("Nendoroid com esta numeração não cadastrada.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Delete_DeveDeletarNendoroid()
        {
            var request = "1583";

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(true);

            var retorno = await _controller.Delete(request) as ObjectResult;
            var responseBody = retorno?.Value as OkResponse;

            Assert.Equal(StatusCodes.Status200OK, retorno?.StatusCode);
            Assert.Equal("Nendoroid deletada com sucesso.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Put_DeveRetornarNendoroidNaoCadastrada()
        {
            var request = new NendoroidRequest();

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(false);

            var retorno = await _controller.Put(request) as ObjectResult;
            var responseBody = retorno?.Value as NotFoundResponse;

            Assert.Equal(StatusCodes.Status404NotFound, retorno?.StatusCode);
            Assert.Equal("Nendoroid com esta numeração não cadastrada.", responseBody?.Mensagem);
        }

        [Fact]
        public async Task Put_DeveAtualizarNendoroid()
        {
            var request = new NendoroidRequest
            {
                Imagens =
                [
                    "https://google.com.br/1", "https://google.com.br/2", "https://google.com.br/3"
                ]
            };

            _mocker.GetMock<INendoroidRepository>().Setup(m => m.Any(It.IsAny<string>())).ReturnsAsync(true);

            var retorno = await _controller.Put(request) as ObjectResult;
            var responseBody = retorno?.Value as OkResponse;

            Assert.Equal(StatusCodes.Status200OK, retorno?.StatusCode);
            Assert.Equal("Nendoroid atualizada com sucesso.", responseBody?.Mensagem);
        }
    }
}
