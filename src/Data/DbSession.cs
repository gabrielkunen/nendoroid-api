using System.Data;
using Npgsql;

namespace NendoroidApi.Data;

public sealed class DbSession : IDisposable
{
    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; set; }

    public DbSession(IConfiguration configuration)
    {
        Connection = new NpgsqlConnection(configuration.GetConnectionString("Postgre"));
        Connection.Open();
    }

    public void Dispose() => Connection?.Dispose();
}