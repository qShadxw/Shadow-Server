using System.Runtime.CompilerServices;

namespace Shadow.Utils;

public class ShadowLogger
{
    private static string GetTimeStamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd | HH:mm:ss");
    }
    
    public static void Log(string message, [CallerFilePath] string? filePath = null)
    {
        Console.WriteLine($"[{GetTimeStamp()}] [{Path.GetFileName(filePath)?.Replace(".cs", "")}] {message}");
    }
    
    public static void Debug(string message, [CallerFilePath] string? filePath = null)
    {
        if (!Config.Debug)
        {
            return;
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{GetTimeStamp()}] [{Path.GetFileName(filePath)?.Replace(".cs", "")}] [DEBUG] {message}");
        Console.ResetColor();
    }

    public static void ColourLog(ConsoleColor color, string message, [CallerFilePath] string? filePath = null)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[{GetTimeStamp()}] [{Path.GetFileName(filePath)?.Replace(".cs", "")}] {message}");
        Console.ResetColor();
    }

    public static void Warn(string message, [CallerFilePath] string? filePath = null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{GetTimeStamp()}] [{Path.GetFileName(filePath)?.Replace(".cs", "")}] [WARN] {message}");
        Console.ResetColor();
    }
    
    public static void Error(string message, [CallerFilePath] string? filePath = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{GetTimeStamp()}] [{Path.GetFileName(filePath)?.Replace(".cs", "")}] [ERROR] {message}");
        Console.ResetColor();
    }
}