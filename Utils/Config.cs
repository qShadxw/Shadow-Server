namespace Shadow.Utils;

public sealed class Config
{
    // Program
    public static readonly bool Debug = true;
    public static bool IsServer = false;
    
    // Game
    public static readonly string DefaultIp = "127.0.0.1";
    public static readonly string DefaultPort = "23456";
    
    public static string Ip = "127.0.0.1";
    public static string Port = "23456";

    public const int MaxPlayers = 100;
    public const int TicksPerSecond = 30;
    public const float MsPerTick = 1000/TicksPerSecond;
}