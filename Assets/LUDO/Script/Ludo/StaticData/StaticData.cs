using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData {

    public static string Name;


    #region Photon Event Codes

    public const byte PLAYER_JOINED = 41;
    public const byte GAME_START = 42;
    public const byte RED_DICE = 43;
    public const byte GREEN_DICE = 44;
    public const byte MOVE_RED = 45;
    public const byte MOVE_GREEN = 46;

    #endregion



    public static string GetPlayerName()
    {
        return Name;
    }

    public static void SetPlayerName(string name)
    {
        Name = name;
    }

}
