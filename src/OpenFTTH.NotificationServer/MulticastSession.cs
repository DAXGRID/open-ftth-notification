using Microsoft.Extensions.Logging;
using NetCoreServer;
using System.Net.Sockets;
using System.Text;

namespace OpenFTTH.NotificationServer;

internal sealed class MulticastSession : WsSession
{
    private readonly ILogger<MulticastSession> _logger;

    public MulticastSession(
        WsServer server,
        ILogger<MulticastSession> logger) : base(server)
    {
        _logger = logger;
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        // We encode the message because there has been issues
        // where not the whole message has been send,
        // by always sending it as MultiCastText, it solves the issue.
        var message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        ((WsServer)Server).MulticastText(message);
    }

    public override void OnWsConnected(HttpRequest request)
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
