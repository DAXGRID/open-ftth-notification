using Microsoft.Extensions.Logging;

namespace OpenFTTH.Notification;

internal sealed class NotificationServer
{
    private readonly ILogger<NotificationServer> _logger;

    public NotificationServer(ILogger<NotificationServer> logger)
    {
        _logger = logger;
    }

    public async Task Start()
    {
        _logger.LogInformation("Starting {Name}.", nameof(NotificationServer));
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
