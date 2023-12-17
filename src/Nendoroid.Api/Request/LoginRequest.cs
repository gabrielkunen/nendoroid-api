using System.ComponentModel.DataAnnotations;

namespace NendoroidApi;

public class LoginRequest
{
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(250, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(64, ErrorMessage = "O campo {0} deve possuir um tamanho entre {2} e {1} caracteres.", MinimumLength = 8)]
    public string Senha { get; set; }
}
