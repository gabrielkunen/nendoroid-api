using System.ComponentModel.DataAnnotations;
using NendoroidApi.CustomValidations;
using NendoroidApi.Domain.Models;

namespace NendoroidApi.Request;

/// <summary>
/// Requisição para cadastro de uma nendoroid
/// </summary>
public class NendoroidRequest {

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(150, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(10, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Numeracao { get; set; }

    public decimal Preco { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Serie { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Fabricante { get; set; }

    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Escultor { get; set; }

    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Cooperacao { get; set; }

    public DateTime DataLancamento { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(200, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Url { get; set; }

    [StringLength(500, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Especificacoes { get; set; }

    [TamanhoMaximoArrayString(200, ErrorMessage = "O comprimento máximo de cada string no array do campo {0} é {1} caracteres.")]
    public string[] Imagens { get; set; }

    public static implicit operator Nendoroid(NendoroidRequest request) =>
        new Nendoroid(request.Nome, request.Numeracao, request.Preco, request.Serie,
            request.Fabricante, request.Escultor, request.Cooperacao, request.DataLancamento,
            request.Url, request.Especificacoes);
}