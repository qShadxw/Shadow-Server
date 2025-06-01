using System.Net.Sockets;
using System.Text;
using Shadow.Models;
using Shadow.Utils;

namespace Shadow;

public class Client
{
    public ClientPlayer? Player;
    private static byte[] ReceiveBuffer;
    public bool IsRunning;
    
    public void Run()
    {
        ShadowLogger.Log("Client starting...");
        Player = new("Player", new Connection(new TcpClient(Config.Ip, Int32.Parse(Config.Port))));
        
        ReceiveBuffer = new byte[Player.Connection.Socket.ReceiveBufferSize];
        Player.Connection.NetworkStream.BeginRead(ReceiveBuffer, 0, Player.Connection.Socket.ReceiveBufferSize,
            ReceivedData, null);

        IsRunning = true;

        Thread commandThread = new Thread(CommandLoop);
        commandThread.Start();
        
        while (IsRunning)
        {
            Thread.Sleep(100);
        }
    }

    private void ReceivedData(IAsyncResult result)
    {
        try
        {
            int bytesRead = Player.Connection.NetworkStream.EndRead(result);
            if (bytesRead <= 0)
            {
                CloseConnection();
                return;
            }

            byte[] data = new byte[bytesRead];
            Array.Copy(ReceiveBuffer, data, bytesRead);

            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms);

            Packets.PacketType packetType = (Packets.PacketType)reader.ReadByte();

            int messageLength = reader.ReadInt32();

            byte[] messageBytes = reader.ReadBytes(messageLength);
            string message = Encoding.UTF8.GetString(messageBytes);

            Console.WriteLine($"Received | {packetType}: {message}");

            Player.Connection.NetworkStream.BeginRead(ReceiveBuffer, 0, ReceiveBuffer.Length, ReceivedData, null);
        }
        catch (Exception ex)
        {
            ShadowLogger.Error($"Error receiving data: {ex}");
            CloseConnection();
        }
    }

    private void CommandLoop()
    {
        while (IsRunning)
        {
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            string[] args = input.Split(' ');
            string command = args[0].ToLower();

            switch (command)
            {
                case "stop":
                case "exit":
                    ShadowLogger.ColourLog(ConsoleColor.Yellow, "Shutting down client...");
                    IsRunning = false;
                    break;

                case "msg":
                case "message":
                    string message = string.Join(' ', args.Skip(1));
                    ShadowLogger.ColourLog(ConsoleColor.Cyan, $"[Message]: {message}");
                    Player.Connection.SendMessage(message);
                    break;

                default:
                    ShadowLogger.ColourLog(ConsoleColor.Red, $"Unknown command: {command}");
                    break;
            }
        }
    }
    
    private void CloseConnection()
    {
        ShadowLogger.Log("Client disconnecting...");

        Player?.Connection.Socket.Close();
        Player = null;
        IsRunning = false;
        Environment.Exit(0);
    }
}