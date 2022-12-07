using NetCoreServer;
using System.Net.Sockets;

namespace OpenFTTH.Notification;

internal sealed class EchoSession : TcpSession
{
    public EchoSession(TcpServer server) : base(server) { }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        this.Server.Multicast(buffer);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Session caught an error with code {error}");
    }
}

internal sealed class EchoServer : TcpServer
{
    public EchoServer(string address, int port) : base(address, port)
    {
    }

    protected override TcpSession CreateSession()
    {
        return new EchoSession(this);
    }
}
