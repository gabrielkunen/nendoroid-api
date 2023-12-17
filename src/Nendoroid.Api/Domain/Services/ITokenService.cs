using NendoroidApi.Domain.Models;

namespace NendoroidApi.Domain.Services;

public interface ITokenService
{
    string Gerar(Usuario usuario);
}
