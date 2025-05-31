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
            Debug.Log($"������ ��-��� ��� {type}, ���������: {size}");
            miniGames[type].StartGame(type, size, OnMiniGameComplete);
        }
        else
        {
            Debug.LogError($"̳�-��� ��� {type} �� ��������!");
        }
    }

    private void OnMiniGameComplete(bool success)
    {
        Debug.Log($"̳�-��� ���������. ���������: {(success ? "����" : "�������")}");
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