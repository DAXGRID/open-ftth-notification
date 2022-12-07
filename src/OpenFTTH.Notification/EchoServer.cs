using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace OpenFTTH.Notification;

internal sealed class EchoSession : TcpSession
{
    public EchoSession(TcpServer server) : base(server) { }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        // Resend the message back to the client
        SendAsync(buffer, offset, size);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Session caught an error with code {error}");
    }
}

internal sealed class EchoServer : TcpServer
{
    public EchoServer(IPAddress address, int port) : base(address, port) { }

    protected override TcpSession CreateSession()
    {
        return new EchoSession(this);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Server caught an error with code {error}");
    }
}
