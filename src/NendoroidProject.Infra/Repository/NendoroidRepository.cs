using Dapper;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Models;
using NendoroidProject.Infra.Context;

namespace NendoroidProject.Infra.Repository;

public class NendoroidRepository : INendoroidRepository
{
    private DbSession _session;
    private const string _tabelaNendoroid = "nendoroid";
    private const string _tabelaNendoroidImagens = "nendoroidimagens";

    public NendoroidRepository(DbSession session)
    {
        _session = session;
    }

    public async Task<int> Add(Nendoroid nendoroid)
    {
        var comando = $@"INSERT INTO {_tabelaNendoroid} (nome, numeracao, preco, serie, fabricante, escultor, cooperacao, datalancamento, url, datacadastro, especificacoes)
                        VALUES (@nome, @numeracao, @preco, @serie, @fabricante, @escultor, @cooperacao, @datalancamento, @url, @datacadastro, @especificacoes) RETURNING id";

        var argumentos = new
        {
            nome = nendoroid.Nome,
            numeracao = nendoroid.Numeracao,
            preco = nendoroid.Preco,
            serie = nendoroid.Serie,
            fabricante = nendoroid.Fabricante,
            escultor = nendoroid.Escultor,
            cooperacao = nendoroid.Cooperacao,
            datalancamento = nendoroid.DataLancamento,
            url = nendoroid.Url,
            datacadastro = nendoroid.DataCadastro,
            especificacoes = nendoroid.Especificacoes
        };

        var id = await _session.Connection.ExecuteScalarAsync<int>(comando, argumentos, _session.Transaction);

        return id;
    }

    public async Task<bool> Any(string numeracao)
    {
        var comando = $"SELECT count(*) FROM {_tabelaNendoroid} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var quantidade = await _session.Connection.QueryFirstAsync<int>(comando, argumentos, _session.Transaction);
        return quantidade > 0;
    }

    public async Task<Nendoroid?> Get(string numeracao)
    {
        var comando = $"SELECT * FROM {_tabelaNendoroid} WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var nendoroid = await _session.Connection.QueryFirstOrDefaultAsync<Nendoroid>(comando, argumentos, _session.Transaction);

        return nendoroid;
    }

    public async Task Delete(string numeracao)
    {
        var comando = $"DELETE FROM {_tabelaNendoroid} WHERE NUMERACAO=(@numeracao)";

        var argumentos = new {  numeracao  };

        await _session.Connection.ExecuteAsync(comando, argumentos, _session.Transaction);
    }

    public async Task<int> Update(Nendoroid nendoroid)
    {
        var comando = $@"UPDATE {_tabelaNendoroid}
                    SET nome = @nome, preco = @preco, serie = @serie, fabricante = @fabricante, escultor = @escultor, cooperacao = @cooperacao, datalancamento = @datalancamento,
                    url = @url, dataalteracao = @dataalteracao, especificacoes = @especificacoes WHERE numeracao = @numeracao RETURNING id";

        var argumentos = new
        {
            nome = nendoroid.Nome,
            preco = nendoroid.Preco,
            serie = nendoroid.Serie,
            fabricante = nendoroid.Fabricante,
            escultor = nendoroid.Escultor,
            cooperacao = nendoroid.Cooperacao,
            datalancamento = nendoroid.DataLancamento,
            url = nendoroid.Url,
            dataalteracao = nendoroid.DataAlteracao,
            especificacoes = nendoroid.Especificacoes,
            numeracao = nendoroid.Numeracao
        };

        var id = await _session.Connection.ExecuteScalarAsync<int>(comando, argumentos, _session.Transaction);

        return id;
    }

    public async Task AddImagens(List<NendoroidImagens> nendoroidImagens)
    {
        var comando = $@"INSERT INTO {_tabelaNendoroidImagens} (idnendoroid, url)
                            VALUES (@idnendoroid, @url)";

        var argumentos = new List<object>();

        foreach(var imagem in nendoroidImagens)
            argumentos.Add(new {idnendoroid = imagem.IdNendoroid, url = imagem.Url});

        await _session.Connection.ExecuteAsync(comando, argumentos,_session.Transaction);
    }

    public async Task DeleteImagens(int idNendoroid)
    {
        var comando = $"DELETE FROM {_tabelaNendoroidImagens} WHERE idnendoroid = (@idNendoroid)";

        var argumentos = new {  idNendoroid  };

        await _session.Connection.ExecuteAsync(comando, argumentos, _session.Transaction);
    }
}