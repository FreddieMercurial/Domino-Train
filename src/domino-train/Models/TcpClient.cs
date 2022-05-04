using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;

namespace DominoTrain.Models;

[UnsupportedOSPlatform(platformName: "browser")]
public sealed class TcpClient
{
    private readonly Socket socket;

    public TcpClient()
    {
        this.isConnected = false;
        this.socket = new Socket(
            addressFamily: AddressFamily.InterNetwork,
            socketType: SocketType.Stream,
            protocolType: ProtocolType.Tcp);
    }

    public bool isConnected { get; private set; }

    public bool ConnectServer(string ip, int port)
    {
        var ipAddress = IPAddress.Parse(ipString: ip);
        var ipEndPoint = new IPEndPoint(address: ipAddress,
            port: port);
        try
        {
            this.socket.Connect(remoteEP: ipEndPoint);
            this.isConnected = true;
            return true;
        }
        catch
        {
            this.isConnected = false;
            return false;
        }
    }

    public bool SendMessage(string message)
    {
        if (!this.isConnected) return false;

        try
        {
            this.socket.Send(buffer: Encoding.ASCII.GetBytes(s: message));
            return true;
        }
        catch
        {
            this.isConnected = false;
            return false;
        }
    }

    public void Disconnect()
    {
        this.socket.Disconnect(reuseSocket: true);
    }
}