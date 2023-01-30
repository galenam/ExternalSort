namespace ConsoleApp1;

public sealed class Settings
{
    public string Path { get; set; }
    public string NameSource { get; set; }
    public string NameDestination { get; set; }
    public int CountLinesInBlock { get; set; }
    
    public Settings()
    {
    }
}