using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class CustomResponse<T> : BaseResponse {
    public T? Data { get; set; }

    public CustomResponse(HttpStatusCode statusCode, string mensagem) : base (statusCode, mensagem)
    {
    }
    public CustomResponse(HttpStatusCode statusCode, string mensagem, T data) : base (statusCode, mensagem)
    {
        Data = data;
    }
}