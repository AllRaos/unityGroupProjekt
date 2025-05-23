using UnityEngine;
using static PlayerController;

public interface IMiniGame
{
    void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete);
}