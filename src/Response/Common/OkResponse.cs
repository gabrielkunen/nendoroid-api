using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class OkResponse : BaseResponse 
{
    public OkResponse(string mensagem) : base(HttpStatusCode.OK, mensagem) {}
}