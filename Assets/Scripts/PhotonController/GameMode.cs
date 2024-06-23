using UnityEngine;

public enum GameType
{
    Single,
    Multiplayer
}

[CreateAssetMenu(menuName = "ScriptableObjects/Game Mode", order = 1)]
public class GameMode : ScriptableObject
{
    public GameType gameType;


    public void SinglePlayer()
    {
        gameType = GameType.Single;
    }

    public void MultiPlayer()
    {
        gameType = GameType.Multiplayer;
    }
}
