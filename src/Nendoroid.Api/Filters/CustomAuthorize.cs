using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NendoroidApi.Response.Common;

namespace NendoroidApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthorize : Attribute, IAuthorizationFilter
{
    public string Role { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new JsonResult(new NaoAutenticadoResponse("Não autenticado."))
            {
                StatusCode = 401
            };
            return;
        }

        if (!IsInRole(context))
        {
            context.Result = new JsonResult(new NaoAutorizadoResponse("Não autorizado."))
            {
                StatusCode = 403
            };
            return;
        }
    }

    private bool IsInRole(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.IsInRole(Role))
            return true;

        return false;
    }
}
