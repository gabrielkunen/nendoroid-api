using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class NotFoundResponse : BaseResponse 
{
    public NotFoundResponse(string mensagem) : base(HttpStatusCode.NotFound, mensagem) {}
}