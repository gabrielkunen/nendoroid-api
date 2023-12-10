﻿using System.Globalization;
using Dapper;
using HtmlAgilityPack;
using NendoroidApi.Domain.Models;
using NendoroidWebCrawler;
using Npgsql;

using var httpClient = new HttpClient();
int quantidadeAtualPaginas = await ExtrairQuantidadePaginas();
const string urlGoodSmile = "https://www.goodsmile.info/en/products/category/nendoroid_series/page/";
const string connectionString = "Host=localhost;Username=postgres;Password=;Database=nendoroid";

await ExtrairDados();

async Task ExtrairDados()
{
    for (int i = 1; i <= 1; i++)
    {
        var url = urlGoodSmile + i;
        var html = await httpClient.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

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
    var nome = htmlDocumentNendo.ExtrairNome();
    var numeracao = htmlDocumentNendo.ExtrairNumeracao();
    var serie = htmlDocumentNendo.ExtrairSerie();
    var fabricante = htmlDocumentNendo.ExtrairFabricante();
    var preco = htmlDocumentNendo.ExtrairPreco();
    var dataLancamento = htmlDocumentNendo.ExtrairDataLancamento();
    var escultor = htmlDocumentNendo.ExtrairEscultor();
    var cooperacao = htmlDocumentNendo.ExtrairCooperacao();
    var especificacoes = htmlDocumentNendo.ExtrairEspecificacoes();

    var nendoroid = new Nendoroid(nome, numeracao, Convert.ToInt32(preco), serie, fabricante, escultor, cooperacao, 
        DateTime.ParseExact(dataLancamento, "yyyy/MM", CultureInfo.InvariantCulture), nendoUrl, especificacoes);

    var idNendo = await Add(nendoroid);

    await ExtrairImagensNendoroid(htmlDocumentNendo, idNendo);
}

async Task ExtrairImagensNendoroid(HtmlDocument htmlDocumentNendo, int idNendo)
{
    var nendoroidImagens = new List<NendoroidImagens>();
    var imagens = htmlDocumentNendo.ExtrairImagens();

    if (imagens == null) return;

    foreach (var imagem in imagens)
        nendoroidImagens.Add(new NendoroidImagens(idNendo, "https:" + imagem.Attributes["src"].Value));

    await AddFoto(nendoroidImagens);
}

async Task<int> Add(Nendoroid nendoroid)
{
    var comando = $@"INSERT INTO nendoroid (nome, numeracao, preco, serie, fabricante, escultor, cooperacao, datalancamento, url, datacadastro, especificacoes)
                        VALUES (@nome, @numeracao, @preco, @serie, @fabricante, @escultor, @cooperacao, @datalancamento, @url, @datacadastro, @especificacoes) RETURNING id";

    var argumentos = new
    {
        nome = nendoroid.Nome,
        numeracao = nendoroid.Numeracao,
        preco = nendoroid.Preco,
        serie = nendoroid.Serie,
        fabricante = nendoroid.Fabricante,
        escultor = nendoroid.Escultor,
        cooperacao = nendoroid.Cooperacao,
        datalancamento = nendoroid.DataLancamento,
        url = nendoroid.Url,
        datacadastro = DateTime.UtcNow,
        especificacoes = nendoroid.Especificacoes
    };

    var conexao = new NpgsqlConnection(connectionString);

    var id = await conexao.ExecuteScalarAsync<int>(comando, argumentos);

    return id;
}

async Task AddFoto(List<NendoroidImagens> nendoroidImagens)
{
    var comando = $@"INSERT INTO nendoroidimagens (idnendoroid, url)
                        VALUES (@idnendoroid, @url)";

    var argumentos = new List<object>();

    foreach(var imagem in nendoroidImagens)
        argumentos.Add(new {idnendoroid = imagem.IdNendoroid, url = imagem.Url});

    var conexao = new NpgsqlConnection(connectionString);

    await conexao.ExecuteAsync(comando, argumentos);
}