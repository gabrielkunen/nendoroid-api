using Microsoft.AspNetCore.Mvc.Testing;
using NendoroidProject.Api.Request;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace NendoroidProject.Api.Tests.V1.Controllers
{
    [TestCaseOrderer(
    ordererTypeName: "NendoroidProject.Api.Tests.IntegrationTestsPriorityOrder",
    ordererAssemblyName: "NendoroidProject.Api.Tests")]
    public class NendoroidControllerIntegrationTests : IntegrationTestsBase
    {
        public NendoroidControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact, TestPriority(1)]
        public async Task Get_DeveRetornarNotFound()
        {
            var response = await Client.GetAsync("/v1/nendoroids?numeracao=99999999");
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(2)]
        public async Task Put_DeveRetornarNotFound()
        {
            var request = new NendoroidRequest
            {
                Nome = "Integration Tests Name",
                Numeracao = "99999999",
                Preco = 5000,
                Serie = "Integration Tests Serie",
                Fabricante = "Integration Tests Fabricante",
                Escultor = "Integration Tests Escultor",
                Cooperacao = "Integration Tests Cooperacao",
                DataLancamento = DateTime.UtcNow,
                Url = "www.integrationtests.com.br",
                Especificacoes = "Integration Tests Especificacoes",
                Imagens = ["imagem1", "imagem2"]
            };

            string json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync("/v1/nendoroids", content);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(3)]
        public async Task Delete_DeveRetornarNotFound()
        {
            var response = await Client.DeleteAsync("/v1/nendoroids?numeracao=99999999");
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact, TestPriority(4)]
        public async Task Post_DeveSalvarNendoroidComSucesso()
        {
            var request = new NendoroidRequest
            {
                Nome = "Integration Tests Name",
                Numeracao = "99999999",
                Preco = 5000,
                Serie = "Integration Tests Serie",
                Fabricante = "Integration Tests Fabricante",
                Escultor = "Integration Tests Escultor",
                Cooperacao = "Integration Tests Cooperacao",
                DataLancamento = DateTime.UtcNow,
                Url = "www.integrationtests.com.br",
                Especificacoes = "Integration Tests Especificacoes",
                Imagens = ["imagem1", "imagem2"]
            };

            string json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/v1/nendoroids", content);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact, TestPriority(5)]
        public async Task Post_DeveRetornarConflito()
        {
            var request = new NendoroidRequest
            {
                Nome = "Integration Tests Name",
                Numeracao = "99999999",
                Preco = 5000,
                Serie = "Integration Tests Serie",
                Fabricante = "Integration Tests Fabricante",
                Escultor = "Integration Tests Escultor",
                Cooperacao = "Integration Tests Cooperacao",
                DataLancamento = DateTime.UtcNow,
                Url = "www.integrationtests.com.br",
                Especificacoes = "Integration Tests Especificacoes",
                Imagens = ["imagem1", "imagem2"]
            };

            string json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/v1/nendoroids", content);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact, TestPriority(6)]
        public async Task Get_DeveRetornarOk()
        {
            var response = await Client.GetAsync("/v1/nendoroids?numeracao=99999999");
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(7)]
        public async Task Put_DeveRetornarOk()
        {
            var request = new NendoroidRequest
            {
                Nome = "Integration Tests Name",
                Numeracao = "99999999",
                Preco = 5000,
                Serie = "Integration Tests Serie",
                Fabricante = "Integration Tests Fabricante",
                Escultor = "Integration Tests Escultor",
                Cooperacao = "Integration Tests Cooperacao",
                DataLancamento = DateTime.UtcNow,
                Url = "www.integrationtests.com.br",
                Especificacoes = "Integration Tests Especificacoes",
                Imagens = ["imagem1", "imagem2"]
            };

            string json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync("/v1/nendoroids", content);
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(8)]
        public async Task Delete_DeveRetornarOk()
        {
            var response = await Client.DeleteAsync("/v1/nendoroids?numeracao=99999999");
            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
