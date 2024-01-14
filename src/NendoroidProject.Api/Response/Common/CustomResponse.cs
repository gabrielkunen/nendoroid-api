using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

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