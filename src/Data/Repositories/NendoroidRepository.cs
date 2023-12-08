using Dapper;
using NendoroidApi.Domain.Models;
using NendoroidApi.Domain.Repositories;

namespace NendoroidApi.Data.Repositories;

public class NendoroidRepository : INendoroidRepository
{
    private DbSession _session;
    private const string _nomeTabela = "nendoroid";
    public NendoroidRepository(DbSession session)
    {
        _session = session;
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

        var id = await _session.Connection.ExecuteScalarAsync<int>(comando, argumentos, _session.Transaction);

        return id;
    }

    public async Task<bool> Any(string numeracao)
    {
        var comando = $"SELECT count(*) FROM {_nomeTabela} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var quantidade = await _session.Connection.QueryFirstAsync<int>(comando, argumentos, _session.Transaction);
        return quantidade > 0;
    }

    public async Task<Nendoroid?> Get(string numeracao)
    {
        var comando = $"SELECT * FROM {_nomeTabela} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var nendoroid = await _session.Connection.QueryFirstOrDefaultAsync<Nendoroid>(comando, argumentos, _session.Transaction);

        return nendoroid;
    }

    public async Task Delete(string numeracao)
    {
        var comando = $"DELETE FROM {_nomeTabela} WHERE NUMERACAO=(@numeracao)";

        var argumentos = new {  numeracao  };

        await _session.Connection.ExecuteAsync(comando, argumentos, _session.Transaction);
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

        await _session.Connection.ExecuteAsync(comando, argumentos, _session.Transaction);
    }
}