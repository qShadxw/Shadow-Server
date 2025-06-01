namespace Shadow.Utils;

public class PlayerUtils
{
    private static int _accountCount = 0;

    public static int GetNewestPlayerId()
    {
        return ++_accountCount;
    }
}