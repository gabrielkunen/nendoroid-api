using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class InternalServerErrorResponse : BaseResponse 
{
    public InternalServerErrorResponse(string mensagem) : base(HttpStatusCode.InternalServerError, mensagem) {}
}