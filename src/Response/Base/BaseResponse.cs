using System.Net;
using NendoroidApi.Extensions;

namespace NendoroidApi.Response.Base;

public abstract class BaseResponse {
    public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.OK;
    public bool Sucesso { get; private set; } = false;
    public string Mensagem { get; private set; } = "";
    public DateTime Timestamp { get; private set; }
    public BaseResponse(HttpStatusCode statusCode, string mensagem)
    {
        StatusCode = statusCode;
        Sucesso = statusCode.IsSuccess();
        Mensagem = mensagem;
        Timestamp = DateTime.Now;
    }
}