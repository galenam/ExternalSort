using ConsoleApp1;
using ConsoleApp1.PrepareFile;
using ConsoleApp1.Sort;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();
        var env = hostingContext.HostingEnvironment;
        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configurationRoot = context.Configuration;
        services.AddSingleton<IPrepareFile, PrepareFile>();
        services.AddSingleton<ISortFile, SortFile>();
        
        services.AddOptions<Settings>()
            .Bind(configurationRoot.GetSection(nameof(Settings)));
        
    })
    .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext())
    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;
var prepareFile = provider.GetRequiredService<IPrepareFile>();
var cancellationToken = new CancellationToken();
await prepareFile.Prepare(cancellationToken);
var sortFiles =  provider.GetRequiredService<ISortFile>();
await sortFiles.Sort(cancellationToken);

