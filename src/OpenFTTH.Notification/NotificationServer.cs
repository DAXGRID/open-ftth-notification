using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenFTTH.Notification;

internal sealed class NotificationServer : BackgroundService
{
    private readonly ILogger<NotificationServer> _logger;

    public NotificationServer(ILogger<NotificationServer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {Name}.", nameof(NotificationServer));

        using var server = new EchoServer(IPAddress.Any, 64001)
        {
            OptionNoDelay = true,
            OptionReuseAddress = true,
        };

        server.Start();

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
