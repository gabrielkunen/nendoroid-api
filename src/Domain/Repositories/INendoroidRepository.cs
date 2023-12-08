using NendoroidApi.Domain.Models;

namespace NendoroidApi.Domain.Repositories;

public interface INendoroidRepository
{
    Task Add(Nendoroid nendoroid);
    Task<bool> Any(string numeracao);
    Task<Nendoroid?> Get(string numeracao);
    Task Delete(string numeracao);
}