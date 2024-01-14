using Dapper;
using NendoroidProject.Domain.Interfaces.Repository;
using NendoroidProject.Domain.Models;
using NendoroidProject.Infra.Context;

namespace NendoroidProject.Infra.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private DbSession _session;

    public UsuarioRepository(DbSession session)
    {
        _session = session;
    }

    private const string _tabelaUsuario = "usuario";
    public async Task<bool> Any(string nome)
    {
        var comando = $"SELECT COUNT(*) FROM {_tabelaUsuario} WHERE NOME = @nome";

        var argumentos = new { nome };

        var quantidade = await _session.Connection.QueryFirstAsync<int>(comando, argumentos, _session.Transaction);
        return quantidade > 0;
    }

    public async Task<int> Add(Usuario usuario)
    {
        var comando = $@"INSERT INTO {_tabelaUsuario} (nome, senha, cargo, datacadastro)
                        VALUES (@nome, @senha, @cargo, @datacadastro) RETURNING id";

        var argumentos = new
        {
            nome = usuario.Nome,
            senha = usuario.Senha,
            cargo = usuario.Cargo,
            datacadastro = usuario.DataCadastro
        };

        var id = await _session.Connection.ExecuteScalarAsync<int>(comando, argumentos, _session.Transaction);

        return id;
    }

    public async Task<Usuario?> Get(string nome)
    {
        var comando = $"SELECT * FROM {_tabelaUsuario} WHERE NOME = @nome";

        var argumentos = new { nome };

        var usuario = await _session.Connection.QueryFirstOrDefaultAsync<Usuario>(comando, argumentos, _session.Transaction);

        return usuario;
    }
}
