using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class NaoAutenticadoResponse : BaseResponse 
{
    public NaoAutenticadoResponse(string mensagem) : base(HttpStatusCode.Unauthorized, mensagem) {}
}
