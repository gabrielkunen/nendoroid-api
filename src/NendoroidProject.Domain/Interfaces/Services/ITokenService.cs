using NendoroidProject.Domain.Models;

namespace NendoroidProject.Domain.Interfaces.Services;

public interface ITokenService
{
    string Gerar(Usuario usuario);
    bool SenhaValida(string senhaRequest, string senhaAtualUsuario);
    string HashSenha(string senha, int workFactor);
}
