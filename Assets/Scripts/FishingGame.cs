using UnityEngine;
using System.Collections.Generic;
using static PlayerController;


public class FishingGame : MonoBehaviour
{
    public ReactionMiniGame reactionMiniGame;
    public TensionZoneMinigame tensionZoneMinigame;
    public RiverNavigationMinigame riverNavigationMinigame;
    public SpeedMiniGame speedMiniGame;
    public FinalBossMinigame finalBossMiniGame;
    private Dictionary<FishType, IMiniGame> miniGames;

    void Start()
    {
        miniGames = new Dictionary<FishType, IMiniGame>
        {
            { FishType.Globefish, reactionMiniGame },
            { FishType.GreenAngelfish, tensionZoneMinigame },
            { FishType.Smartfish, riverNavigationMinigame },
            { FishType.GoldFish, speedMiniGame },
            { FishType.Shark, finalBossMiniGame }
        };
    }

    public void StartMiniGame(FishType type, FishSize size)
    {
        if (miniGames.ContainsKey(type))
        {
            Debug.Log($"Запуск міні-гри для {type}, складність: {size}");
            miniGames[type].StartGame(type, size, OnMiniGameComplete);
        }
        else
        {
            Debug.LogError($"Міні-гра для {type} не знайдена!");
        }
    }

    private void OnMiniGameComplete(bool success)
    {
        Debug.Log($"Міні-гра завершена. Результат: {(success ? "Успіх" : "Невдача")}");
        if (success)
        {
            GetComponent<PlayerController>().OnMiniGameSuccess();
        }
        else
        {
            Debug.Log("Good luck next time");
        }
    }
}