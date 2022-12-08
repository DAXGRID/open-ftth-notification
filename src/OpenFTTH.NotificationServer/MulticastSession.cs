using Microsoft.Extensions.Logging;
using NetCoreServer;
using System.Net.Sockets;

namespace OpenFTTH.NotificationServer;

internal sealed class MulticastSession : TcpSession
{
    private readonly ILogger<MulticastSession> _logger;

    public MulticastSession(
        TcpServer server,
        ILogger<MulticastSession> logger) : base(server)
    {
        _logger = logger;
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        this.Server.Multicast(buffer);
    }

    protected override void OnConnected()
    {
        _logger.LogInformation("New session connected with {Id}.", this.Id);
    }

    protected override void OnDisconnected()
    {
        _logger.LogInformation("Session disconnected with {Id}.", this.Id);
    }

    protected override void OnError(SocketError error)
    {
        _logger.LogError("Session caught an error with code {Error}", error);
    }
}
