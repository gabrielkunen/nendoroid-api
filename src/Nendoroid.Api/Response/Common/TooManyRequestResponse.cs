using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi;

public class TooManyRequestResponse : BaseResponse 
{
    public TooManyRequestResponse(string mensagem) : base(HttpStatusCode.TooManyRequests, mensagem) {}
}
