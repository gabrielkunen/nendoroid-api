using NendoroidProject.Domain.Models;

namespace NendoroidProject.Domain.Interfaces.Repository;
public interface IUsuarioRepository
{
    Task<bool> Any(string nome);
    Task<int> Add(Usuario usuario);
    Task<Usuario?> Get(string nome);
}
