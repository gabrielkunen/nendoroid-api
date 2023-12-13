using NendoroidApi.Domain.Models;

namespace NendoroidApi.Domain.Repositories;

public interface INendoroidRepository
{
    Task<int> Add(Nendoroid nendoroid);
    Task<bool> Any(string numeracao);
    Task<Nendoroid?> Get(string numeracao);
    Task Delete(string numeracao);
    Task<int> Update(Nendoroid nendoroid);
    Task AddImagens(List<NendoroidImagens> nendoroidImagens);
    Task DeleteImagens(int idNendoroid);
}