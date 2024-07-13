using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData {

    public static string Name;



    public static string GetPlayerName()
    {
        return Name;
    }

    public static void SetPlayerName(string name)
    {
        Name = name;
    }

}
