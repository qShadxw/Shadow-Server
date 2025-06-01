using Shadow.Utils;

namespace Shadow.Models;

public class ClientPlayer(string playerName, Connection connection)
{
    // Networking
    public Connection Connection { get; set; } = connection;

    // Player Details
    public int PlayerId { get; set; } = PlayerUtils.GetNewestPlayerId();
    public string PlayerName { get; set; } = playerName;

    // Client Details
    private bool IsPlaying { get; set; } = false;
}