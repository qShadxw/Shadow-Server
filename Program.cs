using System.Net.Sockets;
using CommandLine;
using Shadow.Models;
using Shadow.Utils;

namespace Shadow;

public class Program
{
    private class Options
    {
        [Option('s', "server", Required = false, HelpText = "Run as server")]
        public bool Server { get; set; }
        
        [Option('i', "ip", Required = false, HelpText = "Ip to connect to")]
        public string? Ip { get; set; }
        
        [Option('p', "port", Required = false, HelpText = "Port to connect to")]
        public string? Port { get; set; }
    }
    
    public static void Main(string[] args)
    {
        ShadowLogger.Debug("Starting...");
        ShadowLogger.Debug("Parsing Arguments...");

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(option =>
            {
                Config.IsServer = option.Server;
                Config.Ip = option.Ip ?? Config.DefaultIp;
                Config.Port = option.Port ?? Config.DefaultPort;
            });

        if (Config.IsServer)
        {
            StartServer();

            return;
        }

        Client client = new();
        client.Run();
    }

    private static void StartServer()
    {
        Server server = new();
        server.Run();
    }
}