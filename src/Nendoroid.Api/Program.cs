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
        Description = "Nendoroid API",
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
        Description = "JWT",  
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
