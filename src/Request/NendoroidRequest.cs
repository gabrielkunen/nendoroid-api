using System.ComponentModel.DataAnnotations;
using NendoroidApi.Domain.Models;

namespace NendoroidApi.Request;

public class NendoroidRequest {

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Nome { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(10, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Numeracao { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Preco { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Serie { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Fabricante { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Escultor { get; set; }
    [StringLength(100, ErrorMessage = "O campo {0} não pode possuir mais que {1} caracteres.")]
    public string Cooperacao { get; set; }
    public DateTime DataLancamento { get; set; }

    public static implicit operator Nendoroid(NendoroidRequest request) =>
        new Nendoroid(request.Nome, request.Numeracao, request.Preco, request.Serie,
            request.Fabricante, request.Escultor, request.Cooperacao, request.DataLancamento);
}