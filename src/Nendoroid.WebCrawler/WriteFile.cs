namespace NendoroidWebCrawler;

public static class WriteFile
{
    public static void LogWebCrawler(string caminhoDoArquivo, string conteudo)
    {
        using StreamWriter writer = new(caminhoDoArquivo);
        writer.WriteLine(conteudo);
    }
}
