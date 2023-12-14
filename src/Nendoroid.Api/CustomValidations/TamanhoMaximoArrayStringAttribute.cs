using System.ComponentModel.DataAnnotations;

namespace NendoroidApi.CustomValidations;

/// <summary>
/// Validação do tamanho máximo de cada string presente em um array
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class TamanhoMaximoArrayStringAttribute : ValidationAttribute
{
    private readonly int _tamanhoMaximo;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="tamanhoMaximo"></param>
    public TamanhoMaximoArrayStringAttribute(int tamanhoMaximo)
    {
        _tamanhoMaximo = tamanhoMaximo;
    }
    
    /// <inheritdoc />
    public override bool IsValid(object value)
    {
        if (value is string[] array)
            if (array.Any(s => s.Length > _tamanhoMaximo))
                return false;

        return true;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, _tamanhoMaximo);
    }
}
