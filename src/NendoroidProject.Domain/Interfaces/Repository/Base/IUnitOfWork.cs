namespace NendoroidProject.Domain.Interfaces.Repository.Base;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}