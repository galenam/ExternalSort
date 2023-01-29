using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp1.Sort;

public sealed class SortFile : ISortFile
{
    private readonly Settings _settings;
    private readonly ILogger<SortFile> _logger; 
    
    public SortFile(
        IOptions<Settings> options,
        ILogger<SortFile> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task Sort()
    {
        var pathToRead = $"{_settings.Path}/{_settings.Name}";
        if (!File.Exists(pathToRead))
        {
            _logger.LogError("file source not exists, path {PathToRead}", pathToRead);
        }
        await using var fStreamRead = File.OpenRead(pathToRead);
        using var streamReader = new StreamReader(fStreamRead, Encoding.UTF8);
        await using var streamWriter = new StreamWriter(pathToRead);

        while (!streamReader.EndOfStream)
        {
            var block1 = new string[_settings.CountLinesInBlock];
            for (var i = 0; i < block1.Length; i++)
            {
                var line = await streamReader.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                block1[i] = line;
            }

            Array.Sort(block1);
            await streamWriter.WriteAsync(string.Join(string.Empty, block1));
        }
    }
}