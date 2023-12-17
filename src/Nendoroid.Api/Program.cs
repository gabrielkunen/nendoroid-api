using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NendoroidApi;
using NendoroidApi.Data;
using NendoroidApi.Data.Repositories;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Domain.Repositories.Base;
using NendoroidApi.Domain.Services;
using NendoroidApi.Middlewares;
using NendoroidApi.Response.Base;
using NendoroidApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo{
        Version = "v1",
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
                        "O código da API pode ser visualizado no <a href=\"https://github.com/gabrielkunen/nendoroid-api\" target=\"_blank\">GitHub</a>",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Mariano Kunen",
            Url = new Uri("https://www.linkedin.com/in/gabriel-mariano-kunen-02563a195/")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {  
        Name = "Authorization",  
        Type = SecuritySchemeType.ApiKey,  
        Scheme = "oauth2",  
        In = ParameterLocation.Header,  
        Description = "Json Web Token (JWT)",  
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.InvalidModelStateResponseFactory = ModelValidationErrorResponse.GerarModelValidationErrorResponse);

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["ChaveToken"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(); 

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(policyName: "ApiBlock", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        TooManyRequestResponse responseContent;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            responseContent = new TooManyRequestResponse($"Você excedeu o máximo de requests no momento. Tente novamente em {retryAfter.TotalSeconds} segundo(s).");
        }
        else
        {
            responseContent = new TooManyRequestResponse("Você excedeu o máximo de requests no momento. Tente novamente mais tarde.");
        }

        var jsonResult = new JsonResult(responseContent);
        await jsonResult.ExecuteResultAsync(new ActionContext
        {
            HttpContext = context.HttpContext
        });
    };
});


builder.Services.AddScoped<DbSession>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<INendoroidRepository, NendoroidRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
