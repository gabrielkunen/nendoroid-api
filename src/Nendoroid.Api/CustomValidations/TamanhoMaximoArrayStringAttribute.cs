using System.ComponentModel.DataAnnotations;

namespace NendoroidApi.CustomValidations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class TamanhoMaximoArrayStringAttribute : ValidationAttribute
{
    private readonly int _tamanhoMaximo;

    public TamanhoMaximoArrayStringAttribute(int tamanhoMaximo)
    {
        _tamanhoMaximo = tamanhoMaximo;
    }

    public override bool IsValid(object value)
    {
        if (value is string[] array)
            if (array.Any(s => s.Length > _tamanhoMaximo))
                return false;

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, _tamanhoMaximo);
    }
}
