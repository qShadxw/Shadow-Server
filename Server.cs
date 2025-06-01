using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Shadow.Models;
using Shadow.Utils;

namespace Shadow;

class Server
{
    public Dictionary<int, ServerPlayer> Players = new();
    private TcpListener? _socket;
    private bool _running;
    
    public void Run()
    {
        ShadowLogger.Log($"Server starting on: {Config.Ip}:{Config.Port}.");
        
        _socket = new TcpListener(IPAddress.Parse(Config.Ip), Int32.Parse(Config.Port));
        _socket.Start();
        
        _running = true;
        
        _socket.BeginAcceptTcpClient(ClientConnected, null);

        DateTime _lastLoop =  DateTime.Now;
        DateTime _nextLoop = CalculateTick(_lastLoop);
        
        ShadowLogger.ColourLog(ConsoleColor.Green, "Server Started.");
        
        Thread commandThread = new Thread(CommandLoop);
        commandThread.Start();
        
        while (_running)
        {
            while (_nextLoop < DateTime.Now)
            {
                _lastLoop = _nextLoop;
                _nextLoop = CalculateTick(_lastLoop);

                if (_nextLoop > DateTime.Now)
                {
                    Thread.Sleep(_nextLoop - DateTime.Now);
                }
            }
        }
        
        ShadowLogger.ColourLog(ConsoleColor.Red, "Server stopped.");
    }

    private DateTime CalculateTick(DateTime lastTick)
    {
        return lastTick.AddMilliseconds(Config.MsPerTick);
    }

    private void ClientConnected(IAsyncResult result)
    {
        if (Players.Count >= Config.MaxPlayers)
        {
            ShadowLogger.Error($"ServerCount: {Players.Count}");
            ShadowLogger.Error("Server full.");

            return;
        }
        
        TcpClient? client = _socket?.EndAcceptTcpClient(result);

        if (client == null)
        {
            ShadowLogger.Error("Client could not be accepted, possibly null?");

            return;
        }
        
        client.NoDelay = true;
        _socket?.BeginAcceptTcpClient(ClientConnected, null);
        ShadowLogger.ColourLog(ConsoleColor.Green, $"Incoming connection from {client.Client.RemoteEndPoint}.");

        for (int i = 1; i <= Config.MaxPlayers; i++)
        {
            if (!Players.ContainsKey(i))
            {
                Players[i] = new ServerPlayer(Guid.NewGuid().ToString(), client);
                break;
            }
        }
        
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        
        stream.BeginRead(buffer, 0, buffer.Length, ar => HandleClientData(ar, client, buffer), null);
    }
    
    private void HandleClientData(IAsyncResult ar, TcpClient client, byte[] buffer)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            int bytesRead = stream.EndRead(ar);

            if (bytesRead <= 0)
            {
                ShadowLogger.Log("Client disconnected.");
                client.Close();
                return;
            }

            using MemoryStream ms = new(buffer, 0, bytesRead);
            using BinaryReader reader = new(ms);

            Packets.PacketType packetType = (Packets.PacketType)reader.ReadByte();
            int messageLength = reader.ReadInt32();
            string message = Encoding.UTF8.GetString(reader.ReadBytes(messageLength));

            ShadowLogger.Log($"Received {packetType} from client: {message}");

            stream.BeginRead(buffer, 0, buffer.Length, ar => HandleClientData(ar, client, buffer), null);
        }
        catch (Exception ex)
        {
            ShadowLogger.Error($"Error reading from client: {ex}");
            client.Close();
        }
    }
    
    private void CommandLoop()
    {
        while (_running)
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
                    ShadowLogger.ColourLog(ConsoleColor.Yellow, "Shutting down server...");
                    _running = false;
                    break;

                case "bc":
                case "broadcast":
                    string message = string.Join(' ', args.Skip(1));
                    ShadowLogger.ColourLog(ConsoleColor.Cyan, $"[Broadcast]: {message}");
                    
                    foreach (ServerPlayer player in Players.Values)
                    {
                        player.SendMessage(message);
                    }
                    break;

                default:
                    ShadowLogger.ColourLog(ConsoleColor.Red, $"Unknown command: {command}");
                    break;
            }
        }
    }

}