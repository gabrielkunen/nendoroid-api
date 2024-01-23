using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace NendoroidProject.Api.Tests
{
    public abstract class IntegrationTestsBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient Client;
        public IntegrationTestsBase(WebApplicationFactory<Program> factory) 
        {
            Client = factory.WithWebHostBuilder(builder => 
            {
           }).CreateClient();
        }
    }
}
