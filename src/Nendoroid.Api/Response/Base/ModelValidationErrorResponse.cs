using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace NendoroidApi.Response.Base;

public class ModelValidationErrorResponse : BaseResponse {

    public List<string> Erros { get; set; } = [];

    public ModelValidationErrorResponse(HttpStatusCode statusCode, string mensagem) : base(statusCode, mensagem){}

    public static IActionResult GerarModelValidationErrorResponse(ActionContext context)
    {
        var modelValidationErrorResponse = new ModelValidationErrorResponse(HttpStatusCode.BadRequest, "A requisição enviada está inválida.");
        
        var erros = context.ModelState.AsEnumerable();

        foreach (var erro in erros)
            foreach (var inner in erro.Value!.Errors)
                modelValidationErrorResponse.Erros.Add(inner.ErrorMessage);

        return new BadRequestObjectResult(modelValidationErrorResponse);
    }
}