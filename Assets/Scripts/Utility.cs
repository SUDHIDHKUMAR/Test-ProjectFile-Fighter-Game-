
using UnityEngine;

public enum GameTeam
{
    RED_TEAM,
    BLUE_TEAM
}
public enum PlayerSide
{
    Left,
    Right
}
public static class Utility
{
    public static GameTeam teamChoosed;
    public static PlayerSide side;
    internal static float health;

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

    public static PlayerSide GetOppositeSide(PlayerSide side)
    {
        if (side == PlayerSide.Left)
            return PlayerSide.Right;
        else return PlayerSide.Left;
    }
    public static GameTeam GetOppositeTeam(GameTeam team)
    {
        if (team == GameTeam.BLUE_TEAM)
            return GameTeam.RED_TEAM;
        else return GameTeam.BLUE_TEAM;
    }

    public static Color GetColorWithTeam(GameTeam team)
    {
        if (team == GameTeam.BLUE_TEAM)
            return Color.blue;
        else return Color.red;
    }
}
