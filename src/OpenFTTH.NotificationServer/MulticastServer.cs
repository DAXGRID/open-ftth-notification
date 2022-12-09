using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using NetCoreServer;

namespace OpenFTTH.NotificationServer;

internal sealed class MulticastServer : WsServer
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<MulticastServer> _logger;

    public MulticastServer(
        string address,
        int port,
        ILoggerFactory loggerFactory) : base(address, port)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<MulticastServer>();
    }

    protected override TcpSession CreateSession()
    {
        return new MulticastSession(
            this,
            _loggerFactory.CreateLogger<MulticastSession>());
    }

    protected override void OnError(SocketError error)
    {
        _logger.LogError("{Error}", error);
    }
}
