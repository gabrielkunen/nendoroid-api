using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace NendoroidProject.Infra.Context;

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