using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenFTTH.Notification;

internal sealed class NotificationServer : BackgroundService
{
    private readonly ILogger<NotificationServer> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public NotificationServer(
        ILogger<NotificationServer> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string HOST_ADDRESS = "127.0.0.1";
        const int IP = 64000;

        _logger.LogInformation(
            "Starting {Name} on {Addresss} and {Port}.",
            nameof(NotificationServer),
            HOST_ADDRESS,
            IP);

        using var server = new MulticastServer(HOST_ADDRESS, IP, _loggerFactory)
        {
            OptionNoDelay = true,
            OptionReuseAddress = true,
        };

        server.Start();

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
