using System.Net;

namespace NendoroidApi.Extensions;

public static class HttpExtension
{
    public static bool IsSuccess(this HttpStatusCode statusCode) => 
        new HttpResponseMessage(statusCode).IsSuccessStatusCode;
}