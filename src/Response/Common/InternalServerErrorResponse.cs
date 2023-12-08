using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class InternalServerErrorResponse : BaseResponse 
{
    public InternalServerErrorResponse(string mensagem) : base(HttpStatusCode.InternalServerError, mensagem) {}
}