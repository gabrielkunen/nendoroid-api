using Dapper;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;
using Npgsql;

namespace NendoroidApi.Data.Repositories;

public class NendoroidRepository : INendoroidRepository
{
    private NpgsqlConnection connection;
    private const string _nomeTabela = "nendoroid";
    public NendoroidRepository(IConfiguration configuration)
    {
        connection = new NpgsqlConnection(configuration.GetConnectionString("Postgre"));
        connection.Open();
    }

    public async Task Add(Nendoroid nendoroid)
    {
        var comando = $"INSERT INTO {_nomeTabela} (nome, numeracao, preco, serie, fabricante, escultor, cooperacao, datalancamento)"+
            "VALUES (@nome, @numeracao, @preco, @serie, @fabricante, @escultor, @cooperacao, @datalancamento)";

        var argumentos = new
        {
            nome = nendoroid.Nome,
            numeracao = nendoroid.Numeracao,
            preco = nendoroid.Preco,
            serie = nendoroid.Serie,
            fabricante = nendoroid.Fabricante,
            escultor = nendoroid.Escultor,
            cooperacao = nendoroid.Cooperacao,
            datalancamento = nendoroid.DataLancamento
        };

        await connection.ExecuteAsync(comando, argumentos);
    }

    public async Task<bool> Any(string numeracao)
    {
        var comando = $"SELECT count(*) FROM {_nomeTabela} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var quantidade = await connection.QueryFirstAsync<int>(comando, argumentos);
        return quantidade > 0;
    }

    public async Task<Nendoroid?> Get(string numeracao)
    {
        var comando = $"SELECT * FROM {_nomeTabela} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var nendoroid = await connection.QueryFirstOrDefaultAsync<Nendoroid>(comando, argumentos);

        return nendoroid;
    }

    public async Task Delete(string numeracao)
    {
        var comando = $"DELETE FROM {_nomeTabela} WHERE NUMERACAO=(@numeracao)";

        var argumentos = new {  numeracao  };

        await connection.ExecuteAsync(comando, argumentos);
    }
}