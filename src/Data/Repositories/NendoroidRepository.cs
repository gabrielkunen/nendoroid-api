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

    public async Task<int> Add(Nendoroid nendoroid)
    {
        var comando = $@"INSERT INTO {_nomeTabela} (nome, numeracao, preco, serie, fabricante, escultor, cooperacao, datalancamento)
                        VALUES (@nome, @numeracao, @preco, @serie, @fabricante, @escultor, @cooperacao, @datalancamento) RETURNING id";

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

        var id = await connection.ExecuteScalarAsync<int>(comando, argumentos);

        return id;
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

    public async Task Update(Nendoroid nendoroid)
    {
        var comando = $@"UPDATE {_nomeTabela}
                    SET nome = @nome, preco = @preco, serie = @serie, fabricante = @fabricante, cooperacao = @cooperacao, datalancamento = @datalancamento
                    WHERE numeracao = @numeracao";

        var argumentos = new
        {
            nome = nendoroid.Nome,
            preco = nendoroid.Preco,
            serie = nendoroid.Serie,
            fabricante = nendoroid.Fabricante,
            cooperacao = nendoroid.Cooperacao,
            datalancamento = nendoroid.DataLancamento,
            numeracao = nendoroid.Numeracao
        };

        await connection.ExecuteAsync(comando, argumentos);
    }
}