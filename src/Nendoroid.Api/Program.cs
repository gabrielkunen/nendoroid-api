using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NendoroidApi.Data;
using NendoroidApi.Data.Repositories;
using NendoroidApi.Domain.Repositories;
using NendoroidApi.Domain.Repositories.Base;
using NendoroidApi.Middlewares;
using NendoroidApi.Response.Base;

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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.InvalidModelStateResponseFactory = ModelValidationErrorResponse.GerarModelValidationErrorResponse);

builder.Services.AddScoped<DbSession>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<INendoroidRepository, NendoroidRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
