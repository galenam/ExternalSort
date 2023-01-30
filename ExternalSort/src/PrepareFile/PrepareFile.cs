using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp1.PrepareFile;

public sealed class PrepareFile : IPrepareFile
{
    private readonly Settings _settings;
    private readonly ILogger<PrepareFile> _logger;
    
    public PrepareFile(
        IOptions<Settings> options,
        ILogger<PrepareFile> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task Prepare(CancellationToken cancellationToken)
    {
        var pathToRead = $"{_settings.Path}/{_settings.NameSource}";
        if (!File.Exists(pathToRead))
        {
            _logger.LogError($"file source not exists, path {pathToRead}");
        }

        var randomPostfix = new Random().NextInt64();
        var pathToWrite = $"{_settings.Path}/{_settings.NameSource}{randomPostfix}";
        if (File.Exists(pathToWrite))
        {
            _logger.LogError($"file destination exists, path {pathToWrite}");
        }

        await using var fStreamRead = File.OpenRead(pathToRead);
        await using var fStreamWrite = File.OpenWrite(pathToWrite);
        var streamReaderRead = new StreamReader(fStreamRead, Encoding.UTF8);

        while (await streamReaderRead.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            var lineToWrite = line.Length < 10 ? line.PadLeft(10, '0') : line;
            await fStreamWrite.WriteAsync(Encoding.ASCII.GetBytes(lineToWrite), cancellationToken);
        }
        File.Delete(pathToRead);
        File.Move(pathToWrite, pathToRead);
        File.Delete(pathToWrite);
    }
}