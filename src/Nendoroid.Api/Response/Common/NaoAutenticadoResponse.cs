using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class NaoAutenticadoResponse : BaseResponse 
{
    public NaoAutenticadoResponse(string mensagem) : base(HttpStatusCode.Unauthorized, mensagem) {}
}
