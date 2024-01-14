using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class NaoAutorizadoResponse : BaseResponse 
{
    public NaoAutorizadoResponse(string mensagem) : base(HttpStatusCode.Forbidden, mensagem) {}
}
