using System.Net;

namespace NendoroidProject.Domain.Extensions;

public static class HttpExtension
{
    public static bool IsSuccess(this HttpStatusCode statusCode) => 
        new HttpResponseMessage(statusCode).IsSuccessStatusCode;
}