using HtmlAgilityPack;
using NendoroidProject.Domain.Models;
using NendoroidProject.WebCrawler;
using System.Web;

using var httpClient = new HttpClient();
int quantidadeAtualPaginas = await ExtrairQuantidadePaginas();
const string urlGoodSmile = "https://www.goodsmile.info/en/products/category/nendoroid_series/page/";

var paginasPorTask = quantidadeAtualPaginas / 3;

Task[] tasks =
[
    Task.Run(() => ExtrairDados(1, quantidadeAtualPaginas)), 
    Task.Run(() => ExtrairDados(paginasPorTask + 1, paginasPorTask * 2)), 
    Task.Run(() => ExtrairDados(paginasPorTask * 2 + 1, quantidadeAtualPaginas))
];

await Task.WhenAll(tasks);

WriteFile.LogWebCrawler(AppDomain.CurrentDomain.BaseDirectory + "/log/log.txt", "O processamento foi encerrado com sucesso.");

async Task ExtrairDados(int paginaInicial, int paginaFinal)
{
    for (int i = paginaInicial; i <= paginaFinal; i++)
    {
        var url = urlGoodSmile + i;
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        // Páginas vazias
        if (htmlDocument.DocumentNode.SelectNodes("//span[contains(@class, 'hitTtl')]") == null) continue;
        var quantidadeNendoNaPagina = htmlDocument.DocumentNode.SelectNodes("//span[contains(@class, 'hitTtl')]").ToList().Count;
        
        for (int j = 0; j < quantidadeNendoNaPagina; j++)
        {
            string? urlPaginaNendoroid = "";

            try
            {
                var nendoroidBox = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'hitBox')]")[j];
                var linkTagNendoroidBox = nendoroidBox.SelectSingleNode(".//a[@href]");
                
                // Validar se é uma nendoroid mesmo, pois na lista possui Doll e Roupas
                if (linkTagNendoroidBox.ChildNodes.Count == 7)
                {
                    urlPaginaNendoroid = linkTagNendoroidBox.Attributes["href"]?.Value;

                    if(urlPaginaNendoroid == null)
                        continue;

                    var htmlNendo = await httpClient.GetStringAsync(urlPaginaNendoroid);
                    var htmlDocumentNendo = new HtmlDocument();
                    htmlDocumentNendo.LoadHtml(htmlNendo);
                    await ExtrairDadosPaginaNendoroid(htmlDocumentNendo, urlPaginaNendoroid);
                }
            } catch (Exception e)
            {
                WriteFile.LogWebCrawler(AppDomain.CurrentDomain.BaseDirectory + "/log/log.txt", 
                    $"Ocorreu um erro na página {i}: Url da nendoroid: {urlPaginaNendoroid}.\n\nException: {e.Message}. \n\nStackTrace: {e.StackTrace}");
                continue;
            }
        }
    }
}

async Task<int> ExtrairQuantidadePaginas()
{
    var html = await httpClient.GetStringAsync(urlGoodSmile+1);
    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(html);
    return htmlDocument.ExtrairQuantidadePaginas();
}

async Task ExtrairDadosPaginaNendoroid(HtmlDocument htmlDocumentNendo, string nendoUrl)
{
    // Extração dos dados do html
    var nome = htmlDocumentNendo.ExtrairNome();
    var numeracao = htmlDocumentNendo.ExtrairNumeracao();
    var serie = htmlDocumentNendo.ExtrairSerie();
    var fabricante = htmlDocumentNendo.ExtrairFabricante();
    var preco = htmlDocumentNendo.ExtrairPreco();
    var dataLancamento = htmlDocumentNendo.ExtrairDataLancamento();
    var escultor = htmlDocumentNendo.ExtrairEscultor();
    var cooperacao = htmlDocumentNendo.ExtrairCooperacao();
    var especificacoes = htmlDocumentNendo.ExtrairEspecificacoes();

    // Decode da string com marcação html
    nome = HttpUtility.HtmlDecode(nome);
    numeracao = HttpUtility.HtmlDecode(numeracao);
    serie = HttpUtility.HtmlDecode(serie);
    fabricante = HttpUtility.HtmlDecode(fabricante);
    escultor = HttpUtility.HtmlDecode(escultor);
    cooperacao = HttpUtility.HtmlDecode(cooperacao);
    especificacoes = HttpUtility.HtmlDecode(especificacoes);

    var nendoroid = new Nendoroid(nome, numeracao, preco, serie, fabricante, escultor, cooperacao, 
        dataLancamento, nendoUrl, especificacoes);

    var nendoroidJaExiste = await DbPersistence.Any(nendoroid.Numeracao);
    if (nendoroidJaExiste)
    {
        var idNendoroidExclusao = await DbPersistence.BuscarIdNendoroid(nendoroid.Numeracao);
        await DbPersistence.DeleteImagens(idNendoroidExclusao);
        await DbPersistence.Delete(nendoroid.Numeracao);
    }

    var idNendo = await DbPersistence.Add(nendoroid);

    await ExtrairImagensNendoroid(htmlDocumentNendo, idNendo);
}

async Task ExtrairImagensNendoroid(HtmlDocument htmlDocumentNendo, int idNendo)
{
    var nendoroidImagens = new List<NendoroidImagens>();
    var imagens = htmlDocumentNendo.ExtrairImagens();

    if (imagens == null) return;

    foreach (var imagem in imagens)
        nendoroidImagens.Add(new NendoroidImagens(idNendo, "https:" + imagem.Attributes["src"].Value));

    await DbPersistence.AddImagem(nendoroidImagens);
}