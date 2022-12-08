using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Text.Json;

namespace OpenFTTH.NotificationServer;

internal sealed record Setting { }

internal static class Program
{
    public static async Task Main()
    {
        var host = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<Setting>(GetSettings());
                services.AddHostedService<NotificationServerHost>();
                services.AddLogging(l => l.AddSerilog(GetLogger()));
            })
            .Build();

        var loggerFactory = host.Services.GetService<ILoggerFactory>();
        var logger = loggerFactory!.CreateLogger(nameof(Program));

        try
        {
            await host.StartAsync().ConfigureAwait(true);
            await host.WaitForShutdownAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            logger.LogError("{Exception}", ex);
            throw;
        }
        finally
        {
            logger.LogInformation("Shutting down.");
        }
    }

    private static Setting GetSettings()
    {
        var settingsJson = JsonDocument.Parse(File.ReadAllText("appsettings.json"))
            .RootElement.GetProperty("settings").ToString();

        return JsonSerializer.Deserialize<Setting>(settingsJson) ??
            throw new ArgumentException("Could not deserialize appsettings into settings.");
    }

    private static Logger GetLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(new CompactJsonFormatter())
            .CreateLogger();
    }
}
