namespace NendoroidApi.Domain.Models;
public class Nendoroid {
    private int Id { get; }
    public string Nome { get; private set; }
    public string Numeracao { get; private set; }
    public decimal Preco { get; private set; }
    public string Serie { get; private set; }
    public string Fabricante { get; private set; }
    public string Escultor { get; private set; }
    public string Cooperacao { get; private set; }
    public DateTime DataLancamento { get; private set; }
    
    // Dapper
    public Nendoroid(){}
    public Nendoroid(string nome, string numeracao, decimal preco, string serie, string fabricante, string escultor, string cooperacao, DateTime dataLancamento)
    {
        Nome = nome;
        Numeracao = numeracao;
        Preco = preco;
        Serie = serie;
        Fabricante = fabricante;
        Escultor = escultor;
        Cooperacao = cooperacao;
        DataLancamento = dataLancamento;
    }
}