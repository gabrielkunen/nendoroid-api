using Microsoft.Extensions.Options;
using NendoroidProject.Api.V1;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Interfaces.Services;
using NendoroidProject.Infra.Context;
using NendoroidProject.Infra.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NendoroidProject.Api.Configuration
{
    public static class DependencyInjector
    {
        public static void ResolveDependencies(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();
            services.AddScoped<DbSession>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<INendoroidRepository, NendoroidRepository>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<ITokenService, TokenGenerator>();
        }
    }
}
