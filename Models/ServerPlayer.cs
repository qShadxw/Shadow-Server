using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Shadow.Utils;

namespace Shadow.Models;

public class ServerPlayer(string sessionId, TcpClient socket)
{
    public string? SessionId { get; set; } = sessionId;
    public TcpClient? Socket { get; set; } = socket;

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
            ShadowLogger.Error($"Error sending data to Player. [{SessionId}: {ex}]");
        }
    }

    public void SendWelcome(string message)
    {
        if (Socket == null)
        {
            return;
        }
        
        try
        {
            NetworkStream stream = Socket.GetStream();
            List<byte> packetBytes = new List<byte>();
        
            packetBytes.Add((byte)Packets.PacketType.Welcome);
        
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        
            packetBytes.AddRange(BitConverter.GetBytes(messageBytes.Length));
            packetBytes.AddRange(messageBytes);
            
            stream.Write(packetBytes.ToArray(), 0, packetBytes.Count);
        }
        catch (Exception ex)
        {
            ShadowLogger.Error($"Error sending welcome to Player. [{SessionId}: {ex}");
        }
    }
}