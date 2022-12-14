using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenFTTH.NotificationServer;

internal sealed class NotificationServerHost : BackgroundService
{
    private readonly IPAddress HOST_ADDRESS = IPAddress.Any;
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
        _server = new MulticastServer(HOST_ADDRESS, PORT, _loggerFactory);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting {Name} on {Addresss} and {Port}.",
            nameof(NotificationServerHost),
            HOST_ADDRESS,
            PORT);

        _server.Start();

        // We create a file in the tmp folder to indicate that the service is healthy.
        using var _ = File.Create(Path.Combine(Path.GetTempPath(), "healthy"));
        _logger.LogInformation("{Service} is now healthy.", nameof (NotificationServerHost));

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
