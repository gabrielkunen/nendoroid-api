using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi;

public class NaoAutorizadoResponse : BaseResponse 
{
    public NaoAutorizadoResponse(string mensagem) : base(HttpStatusCode.Forbidden, mensagem) {}
}
