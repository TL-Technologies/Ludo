using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData {

    public static string Name;


    #region Photon Event Codes

    public const byte PLAYER_JOINED = 41;
    public const byte GAME_START = 42;

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
