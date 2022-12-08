using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenFTTH.NotificationServer;

internal sealed class NotificationServerHost : BackgroundService
{
    const string HOST_ADDRESS = "127.0.0.1";
    const int PORT = 8000;

    private readonly ILogger<NotificationServerHost> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly MulticastServer _server;

    public NotificationServerHost(
        ILogger<NotificationServerHost> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _server = new MulticastServer(HOST_ADDRESS, PORT, _loggerFactory)
        {
            OptionNoDelay = true,
            OptionReuseAddress = true,
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting {Name} on {Addresss} and {Port}.",
            nameof(NotificationServerHost),
            HOST_ADDRESS,
            PORT);

        _server.Start();

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_server.IsDisposed)
        {
            _logger.LogInformation("Stopping {Name}", nameof(MulticastServer));
            _server.Stop();
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (!_server.IsDisposed)
        {
            _server.Dispose();
        }

        base.Dispose();
    }
}