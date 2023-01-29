namespace ConsoleApp1.PrepareFile;

public interface IPrepareFile
{
    public Task Prepare(CancellationToken cancellationToken);
}