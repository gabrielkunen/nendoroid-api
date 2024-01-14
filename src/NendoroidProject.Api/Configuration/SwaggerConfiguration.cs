using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NendoroidProject.Api.Configuration;

public class SwaggerConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider provider;

    public SwaggerConfiguration(IApiVersionDescriptionProvider provider) => this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Version = description.ApiVersion.ToString(),
            Title = "Nendoroid API",
            Description = $"API para gerenciar as informações das nendoroids até hoje lançadas.\n" +
                            "Uma nendoroid é um boneco de plástico fabricados pela empresa japonesa Good Smile Company desde 2006. " +
                            "Eles retraram algum personagem de anime, mangá, filmes, jogos e etc.\n\n" +
                            "As informações das nendoroids desta API foram retiradas do site oficial da Good Smile inglesa utilizando WebCrawler. \n" +
                            "<h2>Informações disponíveis</h2>" +
                            "<ul>" +
                            "<li><b>Nome</b>: Nome da nendoroid</li>" +
                            "<li><b>Numeração</b>: Numeração da nendoroid, ela começou em 0 e segue incrementando de 1 a 1 até hoje, mas algumas versões especiais recebem a " +
                            "numeração '-DX', significando deluxe, ou outras letras também indicando variações de uma mesma nendoroid.</li>" +
                            "<li><b>Preco</b>: Preco de lançamento da nendoroid em ienes.</li>" +
                            "<li><b>Serie</b>: Nome do anime, mangá, jogo que a nendoroid foi inspirada.</li>" +
                            "<li><b>Fabricante</b>: Fabricante da nendoroid.</li>" +
                            "<li><b>Escultor</b>: Escultor da nendoroid.</li>" +
                            "<li><b>Cooperação</b>: Cooperação da nendoroid.</li>" +
                            "<li><b>DataLancamento</b>: Mês e ano de lançamento da nendoroid.</li>" +
                            "<li><b>Url</b>: Url da página da nendoroid no site da Good Smile Company versão inglês.</li>" +
                            "<li><b>Especificações</b>: Especificações da nendoroid, como tamanho e material que foi utilizado.</li>" +
                            "</ul>\n" +
                            "<h2>Utilização:</h2>" +
                            "Para utilizar a API é necessário criar um usuário no método POST /Usuario, após isso é possível utilizar o método POST /Login para obter o token " +
                            "de autenticação. Este token é o Json Web Token (JWT) e contém as informações do usuário junto com seu cargo (role) na API, cada método possui " +
                            "um nível de cargo para acesso, por padrão ao cadastrar um novo usuário, ele possui o cargo 'comum', só é possivel aumentar o cargo contatando o " +
                            "administrador.\n" +
                            "Os exemplos de requisições, códigos http de retornos possíveis e estrutura de retorno dos endpoints, podem ser visualizados nos endpoints abaixo.\n " +
                            "O código da API pode ser visualizado no <a href=\"https://github.com/gabrielkunen/nendoroid-api\" target=\"_blank\">GitHub</a>\n\n",
            Contact = new OpenApiContact
            {
                Name = "Gabriel Mariano Kunen",
                Url = new Uri("https://www.linkedin.com/in/gabriel-mariano-kunen-02563a195/")
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += "Esta versão da API está depreciada.";
        }

        return info;
    }
}
