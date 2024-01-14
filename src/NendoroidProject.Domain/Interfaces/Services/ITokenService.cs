using NendoroidProject.Domain.Models;

namespace NendoroidProject.Domain.Interfaces.Services;

public interface ITokenService
{
    string Gerar(Usuario usuario);
}
