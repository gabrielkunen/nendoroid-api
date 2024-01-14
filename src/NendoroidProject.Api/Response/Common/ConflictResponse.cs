using System.Net;
using NendoroidProject.Api.Response.Base;

namespace NendoroidProject.Api.Response.Common;

public class ConflictResponse : BaseResponse 
{
    public ConflictResponse(string mensagem) : base(HttpStatusCode.Conflict, mensagem) {}
}