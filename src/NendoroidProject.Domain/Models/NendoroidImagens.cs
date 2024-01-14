namespace NendoroidProject.Domain.Models;
public class NendoroidImagens : Entity 
{
    public int IdNendoroid { get; private set; }
    public string Url { get; private set; }
    
    // Dapper
    public NendoroidImagens(){}
    public NendoroidImagens(int idNendoroid, string url)
    {
        IdNendoroid = idNendoroid;
        Url = url;
    }
}