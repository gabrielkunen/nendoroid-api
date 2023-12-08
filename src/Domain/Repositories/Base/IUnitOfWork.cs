namespace NendoroidApi.Domain.Repositories.Base;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}