using Microsoft.AspNetCore.Mvc;

namespace NendoroidApi.Response.Base;

public class ErroResponse {
    public int StatusCode { get; set; }
    public string Mensagem { get; set; } = "";
    public List<string> Erros { get; set; } = new();
    public DateTime Timestamp { get; set; }

    public ErroResponse(){}
    public ErroResponse(int statusCode, string mensagem)
    {
        StatusCode = statusCode;
        Mensagem = mensagem;
        Timestamp = DateTime.Now;
    }
    public static IActionResult GerarErroResponse(ActionContext context)
    {
        var ErroResponse = new ErroResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Mensagem = "Bad Request",
            Timestamp = DateTime.Now
        };
        
        var erros = context.ModelState.AsEnumerable();

        foreach (var erro in erros)
            foreach (var inner in erro.Value!.Errors)
                ErroResponse.Erros.Add(inner.ErrorMessage);

        return new BadRequestObjectResult(ErroResponse);
    }
}