using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class NotFoundResponse : BaseResponse 
{
    public NotFoundResponse(string mensagem) : base(HttpStatusCode.NotFound, mensagem) {}
}