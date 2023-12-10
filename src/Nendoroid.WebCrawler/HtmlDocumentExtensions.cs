using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace NendoroidWebCrawler;

public static class HtmlDocumentExtensions
{
    public static string ExtrairNome(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//h1[contains(@class, 'title')]").InnerText.Trim();
    }

    public static string ExtrairNumeracao(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'itemNum')]/span").InnerText.Trim();
    }

    public static string ExtrairSerie(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Series']")
            .SelectSingleNode("following-sibling::dd").InnerText.Trim();
    }

    public static string ExtrairFabricante(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Manufacturer']")
            .SelectSingleNode("following-sibling::dd").InnerText.Trim();
    }

    public static string ExtrairPreco(this HtmlDocument htmlDocument)
    {
        var preco = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Price']")
            .SelectSingleNode("following-sibling::dd").InnerText.Trim();
        preco = new Regex(@"[^\d]").Replace(preco, "");
        return preco;
    }

    public static string ExtrairDataLancamento(this HtmlDocument htmlDocument)
    {
        var dataLancamento = htmlDocument.DocumentNode.SelectSingleNode("//dt[contains(@class, 'release_date')]").SelectSingleNode("following-sibling::dd").InnerText.Trim();
        dataLancamento = dataLancamento[..7];
        return dataLancamento;
    }

    public static string ExtrairEscultor(this HtmlDocument htmlDocument)
    {
        string? escultor;

        var escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculptor']");
        if (escultorNode != null)
            escultor = escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        else
            escultor = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculpting/Paintwork']").SelectSingleNode("following-sibling::dd").InnerText.Trim();

        return escultor;
    }

    public static string ExtrairCooperacao(this HtmlDocument htmlDocument)
    {
        string? cooperacao;

        var cooperacaoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Cooperation']");
        if (cooperacaoNode != null)
            cooperacao = cooperacaoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        else
            cooperacao = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()=' Cooperation']").SelectSingleNode("following-sibling::dd").InnerText.Trim();

        return cooperacao;
    }

    public static string ExtrairEspecificacoes(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Specifications']")
            .SelectSingleNode("following-sibling::dd").InnerText.Trim();
    }

    public static HtmlNodeCollection? ExtrairImagens(this HtmlDocument htmlDocument)
    {
        return htmlDocument.DocumentNode.SelectNodes("//img[contains(@class, 'itemImg')]");
    }

    public static int ExtrairQuantidadePaginas(this HtmlDocument htmlDocument)
    {
        var quantidadePaginas = htmlDocument.DocumentNode.SelectSingleNode("//span[contains(@class, 'muted pages')]").InnerText.Trim();
        quantidadePaginas = quantidadePaginas[^2..];
        return Convert.ToInt32(quantidadePaginas);
    }
}
