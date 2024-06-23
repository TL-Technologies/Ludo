using UnityEditor;
using UnityEngine;


public class GameContext : EditorWindow
{
    [MenuItem("Game Mode / Single Player")]
    public static void singlePlayer()
    {
        var data = Resources.Load("GameMode") as GameMode;
        data.SinglePlayer();
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Game Mode / Multi Player")]
    public static void Multiplayer()
    {
        var data = Resources.Load("GameMode") as GameMode;
        data.MultiPlayer();
          EditorUtility.SetDirty(data);
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();
    }
    
    

}
