using UnityEngine;
using System.Collections.Generic;
using static PlayerController;
using System.Collections;

public class MemoryMiniGame : MonoBehaviour, IMiniGame
{
    private List<int> sequence;
    private List<int> playerInput;
    private bool isRunning;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        // Налаштування складності залежно від розміру риби
        int sequenceLength = size == FishSize.Small ? 6 : size == FishSize.Medium ? 4 : 2;
        sequence = new List<int>();
        playerInput = new List<int>();

        // Генеруємо послідовність
        for (int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(Random.Range(0, 4)); 
        }

        isRunning = true;
        Debug.Log($"Міні-гра на пам’ять почалася. Повторіть послідовність: {string.Join(", ", sequence)}");

        // Запускаємо корутину для гри
        StartCoroutine(RunGame(onGameComplete));
    }

    public IEnumerator RunGame(System.Action<bool> onGameComplete)
    {
        float timeLimit = 10f;
        float timer = 0f;

        while (timer < timeLimit && playerInput.Count < sequence.Count)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Alpha0)) playerInput.Add(0);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) playerInput.Add(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) playerInput.Add(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) playerInput.Add(3);

            if (playerInput.Count == sequence.Count)
            {
                bool success = true;
                for (int i = 0; i < sequence.Count; i++)
                {
                    if (playerInput[i] != sequence[i])
                    {
                        success = false;
                        break;
                    }
                }
                Debug.Log(success ? "Успіх! Послідовність правильна!" : "Помилка в послідовності!");
                onGameComplete?.Invoke(success);
                yield break;
            }
            yield return null;
        }

        Debug.Log("Час вийшов!");
        onGameComplete?.Invoke(false);
        PlayerController.Instance.animator.SetTrigger("Hook");
        PlayerController.Instance.EndMiniGame();
    }
}