using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Text.Json;

namespace OpenFTTH.Notification;

internal sealed record Setting { }

internal static class Program
{
    public static async Task Main()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog(GetLogger()))
            .AddSingleton<Setting>(GetSettings())
            .AddSingleton<NotificationServer>()
            .BuildServiceProvider();

        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory!.CreateLogger(nameof(Program));
        var notificationServer = serviceProvider.GetService<NotificationServer>();

        try
        {
            await notificationServer!.Start(token).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            logger.LogError("{Exception}", ex);
            throw;
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
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(new CompactJsonFormatter())
            .CreateLogger();
    }
}
