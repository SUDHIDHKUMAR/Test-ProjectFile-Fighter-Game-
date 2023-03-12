
using UnityEngine;

public enum GameTeam
{
    RED_TEAM,
    BLUE_TEAM
}
public static class PlayerData
{
    public static int PlayerId;
    public static int PlayerName;
    public static GameTeam teamChoosed;

    public static Color GetPlayerColor()
    {

        if (GameTeam.BLUE_TEAM == teamChoosed)
            return Color.blue;
        else
            return Color.red;
    }
    public static Color GetOpponentColor()
    {

        if (GameTeam.BLUE_TEAM == teamChoosed)
            return Color.red;
        else
            return Color.blue;
    }
}
