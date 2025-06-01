using System.Net.Sockets;
using System.Text;
using Shadow.Utils;

namespace Shadow.Models;

public class Connection
{
    public enum State : byte
    {
        NotConnected,
        Connecting,
        Connected
    }

    public State ConnectionState { get; set; }
    public TcpClient Socket { get; set; }
    public NetworkStream NetworkStream { get; set; }
    
    public bool IsDisconnected => ConnectionState == State.NotConnected;
    public bool IsConnecting => ConnectionState == State.Connecting;
    public bool IsConnected => ConnectionState == State.Connected;

    public Connection(TcpClient socket)
    {
        ConnectionState = State.Connecting;
        Socket = socket;
        
        Socket.ReceiveTimeout = 4096;
        Socket.SendBufferSize = 4096;
        
        NetworkStream = Socket.GetStream();
    }
    
    public void SendMessage(string message)
    {
        try
        {
            if (Socket == null)
            {
                return;
            }
            
            NetworkStream stream = Socket.GetStream();
            List<byte> packetBytes = new List<byte>();
        
            packetBytes.Add((byte)Packets.PacketType.Message);
        
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        
            packetBytes.AddRange(BitConverter.GetBytes(messageBytes.Length));
            packetBytes.AddRange(messageBytes);
            
            stream.Write(packetBytes.ToArray(), 0, packetBytes.Count);
        }
        catch (Exception ex)
        {
            ShadowLogger.Error($"Error sending data to Server. [{ex}]");
        }
    }
}