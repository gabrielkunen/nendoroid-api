using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NendoroidProject.Api.Response.Common;
using System.Threading.RateLimiting;

namespace NendoroidProject.Api.Configuration
{
    public static class RateLimitConfiguration
    {
        public static void AddRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter(policyName: "ApiBlock", options =>
                {
                    options.PermitLimit = 5;
                    options.Window = TimeSpan.FromSeconds(10);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    TooManyRequestResponse responseContent;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        responseContent = new TooManyRequestResponse($"Você excedeu o máximo de requests no momento. Tente novamente em {retryAfter.TotalSeconds} segundo(s).");
                    }
                    else
                    {
                        responseContent = new TooManyRequestResponse("Você excedeu o máximo de requests no momento. Tente novamente mais tarde.");
                    }

                    var jsonResult = new JsonResult(responseContent);
                    await jsonResult.ExecuteResultAsync(new ActionContext
                    {
                        HttpContext = context.HttpContext
                    });
                };
            });
        }
    }
}
