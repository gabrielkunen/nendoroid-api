using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class TooManyRequestResponse : BaseResponse 
{
    public TooManyRequestResponse(string mensagem) : base(HttpStatusCode.TooManyRequests, mensagem) {}
}
