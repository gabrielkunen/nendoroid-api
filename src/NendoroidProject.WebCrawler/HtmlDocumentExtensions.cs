using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace NendoroidProject.WebCrawler;

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

    public static int? ExtrairPreco(this HtmlDocument htmlDocument)
    {
        var precoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Price']");
        
        if (precoNode != null)
        {
            var preco = precoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
            preco = new Regex(@"[^\d]").Replace(preco, "");
            return Convert.ToInt32(preco);
        }
        
        return null;
    }

    public static DateTime ExtrairDataLancamento(this HtmlDocument htmlDocument)
    {
        var dataLancamentoString = htmlDocument.DocumentNode.SelectSingleNode("//dt[contains(@class, 'release_date')]")
            .SelectSingleNode("following-sibling::dd").InnerText.Trim();
        var indexPrimeiroNumero = dataLancamentoString.IndexOfAny("0123456789".ToCharArray());
        dataLancamentoString = dataLancamentoString.Substring(indexPrimeiroNumero, 7);
        
        var retornoParse = DateTime.TryParseExact(dataLancamentoString, "yyyy/MM", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, 
            out DateTime dataLancamento);
        if (!retornoParse)
           DateTime.TryParseExact(dataLancamentoString, "MM/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dataLancamento); 

        return dataLancamento;
    }

    public static string ExtrairEscultor(this HtmlDocument htmlDocument)
    {
        string? escultor;

        var escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculptor']");
        if (escultorNode != null)
            return escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();

        escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculpting/Paintwork']");

        if(escultorNode != null)
            return escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        
        escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculpting/Cooperation']");

        if(escultorNode != null)
            return escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();

        escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculptor/Cooperation']");

        if(escultorNode != null)
            return escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        
        escultorNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Planning/Sculpting']");

        if(escultorNode != null)
            return escultorNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        else
            escultor = string.Empty;

        return escultor;
    }

    public static string ExtrairCooperacao(this HtmlDocument htmlDocument)
    {
        string? cooperacao;

        var cooperacaoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Cooperation']");
        if (cooperacaoNode != null)
            return cooperacaoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        
        cooperacaoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()=' Cooperation']");
        
        if (cooperacaoNode != null)
            return cooperacaoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();

        cooperacaoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculpting/Cooperation']");

        if(cooperacaoNode != null)
            return cooperacaoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();

        cooperacaoNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Sculptor/Cooperation']");

        if(cooperacaoNode != null)
            cooperacao = cooperacaoNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        else
            cooperacao = string.Empty;

        return cooperacao;
    }

    public static string? ExtrairEspecificacoes(this HtmlDocument htmlDocument)
    {
        var especificacoesNode = htmlDocument.DocumentNode.SelectSingleNode("//dt[text()='Specifications']");

        if (especificacoesNode != null)
            return especificacoesNode.SelectSingleNode("following-sibling::dd").InnerText.Trim();
        else
            return null;    
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
