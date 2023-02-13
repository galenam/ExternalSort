using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp1.Sort;

public sealed class SortFile : ISortFile
{
    private const short BlockCount = 100;
    private readonly Settings _settings;
    private readonly ILogger<SortFile> _logger;
    private readonly string _pathToRead;

    public SortFile(
        IOptions<Settings> options,
        ILogger<SortFile> logger)
    {
        _settings = options.Value;
        _logger = logger;
        _pathToRead = $"{_settings.Path}/{_settings.NameSource}";
    }

    public async Task Sort(CancellationToken cancellationToken)
    {
        if (!File.Exists(_pathToRead))
        {
            _logger.LogError("file source not exists, path {PathToRead}", _pathToRead);
        }
        
        var streamWriters = new StreamWriter[BlockCount];
        for (var i = 0; i < BlockCount; i++)
        {
            var pathToWrite = $"{_settings.Path}/{i:D2}.txt";
            Console.WriteLine(pathToWrite);
            var streamWriter = new StreamWriter(pathToWrite, false);
            streamWriters[i] = streamWriter;
        }
        
        using var streamReader = new StreamReader(_pathToRead);
        while (await streamReader.ReadLineAsync() is { } line)
        {
            var indexString = line[..2];
            if (int.TryParse(indexString, out var index))
            {
                await streamWriters[index].WriteLineAsync(line);
            }
        }

        foreach (var streamWriter in streamWriters)
        {
            await streamWriter.FlushAsync();
        }

        var pathToDestination = $"{_settings.Path}/{_settings.NameDestination}";
        await using var streamWriterDestination = new StreamWriter(pathToDestination, false);
        for (var i = 0; i < BlockCount; i++)
        {
            var pathToRead = $"{_settings.Path}/{i:D2}.txt";
            var lines = await File.ReadAllLinesAsync(pathToRead, cancellationToken);
            if (lines.Length == 0)
            {
                continue;
            }
            Array.Sort(lines);
            var sBuilder = new StringBuilder(string.Join(Environment.NewLine, lines));
            await streamWriterDestination.WriteAsync(sBuilder, cancellationToken);
        }

        foreach (var streamWriter in streamWriters)
        {
            streamWriter.Close();
            await streamWriter.DisposeAsync();
        }
    }
}