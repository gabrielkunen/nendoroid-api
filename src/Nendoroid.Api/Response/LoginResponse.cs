
namespace NendoroidApi.Response;

public class LoginResponse
{
    public string Nome { get; set; }
    public string Token { get; set; }
    public LoginResponse(string nome, string token)
    {
        Nome = nome;
        Token = token;
    }
}
