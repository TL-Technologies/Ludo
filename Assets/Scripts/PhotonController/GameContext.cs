using UnityEditor;
using UnityEngine;


public class GameContext : EditorWindow
{
    [MenuItem("Game Mode / Single Player")]
    public static void singlePlayer()
    {
        var data = Resources.Load("GameMode") as GameMode;
        data.SinglePlayer();
    }
    
    [MenuItem("Game Mode / Multi Player")]
    public static void Multiplayer()
    {
        var data = Resources.Load("GameMode") as GameMode;
        data.MultiPlayer();
    }
}
