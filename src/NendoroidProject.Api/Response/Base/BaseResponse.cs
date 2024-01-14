using System.Net;
using NendoroidProject.Domain.Extensions;

namespace NendoroidProject.Api.Response.Base;

public abstract class BaseResponse {
    public bool Sucesso { get; private set; } = false;
    public string Mensagem { get; private set; } = "";
    public DateTime Timestamp { get; private set; }
    public BaseResponse(HttpStatusCode statusCode, string mensagem)
    {
        Sucesso = statusCode.IsSuccess();
        Mensagem = mensagem;
        Timestamp = DateTime.UtcNow;
    }
}