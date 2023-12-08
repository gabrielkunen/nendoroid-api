using System.Net;
using NendoroidApi.Response.Base;

namespace NendoroidApi.Response.Common;

public class ConflictResponse : BaseResponse 
{
    public ConflictResponse(string mensagem) : base(HttpStatusCode.Conflict, mensagem) {}
}