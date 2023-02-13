namespace ConsoleApp1.Sort;

public interface ISortFile
{
    Task Sort(CancellationToken cancellationToken);
}