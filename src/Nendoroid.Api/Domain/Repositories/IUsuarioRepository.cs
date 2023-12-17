using NendoroidApi.Domain.Models;

namespace NendoroidApi.Domain.Repositories;
public interface IUsuarioRepository
{
    Task<bool> Any(string nome);
    Task<int> Add(Usuario usuario);
    Task<Usuario?> Get(string nome);
}
