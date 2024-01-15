using Dapper;
using NendoroidProject.Domain.Models;
using Npgsql;

namespace NendoroidProject.WebCrawler;

public static class DbPersistence
{
    const string connectionString = "Host=localhost;Username=postgres;Password=;Database=nendoroid";

    public static async Task<int> Add(Nendoroid nendoroid)
    {
        var comando = $@"INSERT INTO nendoroid (nome, numeracao, preco, serie, fabricante, escultor, cooperacao, datalancamento, url, datacadastro, especificacoes)
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
            datacadastro = DateTime.UtcNow,
            especificacoes = nendoroid.Especificacoes
        };

        var conexao = new NpgsqlConnection(connectionString);

        var id = await conexao.ExecuteScalarAsync<int>(comando, argumentos);

        return id;
    }

    public static async Task<bool> Any(string numeracao)
    {
        var comando = $"SELECT count(*) FROM nendoroid WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var conexao = new NpgsqlConnection(connectionString);

        var quantidade = await conexao.QueryFirstAsync<int>(comando, argumentos);
        return quantidade > 0;
    }

    public static async Task<int> BuscarIdNendoroid(string numeracao)
    {
        var comando = $"SELECT id FROM nendoroid WHERE NUMERACAO = @numeracao";

        var argumentos = new { numeracao };

        var conexao = new NpgsqlConnection(connectionString);

        var idNendoroid = await conexao.QueryFirstAsync<int>(comando, argumentos);
        return idNendoroid;
    }

    public static async Task Delete(string numeracao)
    {
        var comando = $"DELETE FROM nendoroid WHERE NUMERACAO=(@numeracao)";

        var argumentos = new {  numeracao  };

        var conexao = new NpgsqlConnection(connectionString);

        await conexao.ExecuteAsync(comando, argumentos);
    }

    public static async Task DeleteImagens(int idNendoroid)
    {
        var comando = $"DELETE FROM nendoroidimagens WHERE idNendoroid=(@idNendoroid)";

        var argumentos = new { idNendoroid };

        var conexao = new NpgsqlConnection(connectionString);

        await conexao.ExecuteAsync(comando, argumentos);
    }

    public static async Task AddImagem(List<NendoroidImagens> nendoroidImagens)
    {
        var comando = $@"INSERT INTO nendoroidimagens (idnendoroid, url) VALUES (@idnendoroid, @url)";

        var argumentos = new List<object>();

        foreach(var imagem in nendoroidImagens)
            argumentos.Add(new {idnendoroid = imagem.IdNendoroid, url = imagem.Url});

        var conexao = new NpgsqlConnection(connectionString);

        await conexao.ExecuteAsync(comando, argumentos);
    }
}
