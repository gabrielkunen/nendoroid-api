namespace NendoroidWebCrawler;

public static class WriteFile
{
    public static void LogWebCrawler(string caminhoArquivo, string conteudo)
    {
        using StreamWriter writer = File.AppendText(caminhoArquivo);
        writer.WriteLine($"{conteudo}\n----------------------------------------\n");
    }
}
