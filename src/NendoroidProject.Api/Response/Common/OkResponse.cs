using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class OkResponse : BaseResponse 
{
    public OkResponse(string mensagem) : base(HttpStatusCode.OK, mensagem) {}
}