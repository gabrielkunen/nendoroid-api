namespace NendoroidApi.Domain.Models;

public class Usuario
{
    public int Id { get; }
    public string Nome { get; private set; }
    public string Senha { get; private set; }
    public string Cargo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAlteracao { get; private set; }

    // Dapper
    public Usuario(){}
    public Usuario(string nome)
    {
        Nome = nome;
    }

    public void SetarSenhaHashUsuario(string senha) => Senha = senha;
    public void SetarCargoUsuario(string cargo) => Cargo = cargo;
    public void SetarDataCadastroUtcNow() => DataCadastro = DateTime.UtcNow;
}
