using UnityEngine;
using static PlayerController;
using System.Collections;

public class SpeedMiniGame : MonoBehaviour, IMiniGame
{
    private int clickCount;
    private bool isRunning;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        // ������������ ��������� ������� �� ������ ����
        int requiredClicks = size == FishSize.Small ? 20 : size == FishSize.Medium ? 15 : 10;
        clickCount = 0;

        isRunning = true;
        Debug.Log($"̳�-��� �� �������� ��������. �������� ����� {requiredClicks} ���� ������� ������!");

        // ��������� �������� ��� ���
        StartCoroutine(RunGame(requiredClicks, onGameComplete));
    }

    public IEnumerator RunGame(int requiredClicks, System.Action<bool> onGameComplete)
    {
        float timeLimit = 5f;
        float timer = 0f;

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                clickCount++;
                Debug.Log($"��� #{clickCount}");

                if (clickCount >= requiredClicks)
                {
                    Debug.Log("����! �� ������� ������� ������� ����!");
                    onGameComplete?.Invoke(true);
                    yield break;
                }
            }
            yield return null;
        }

        Debug.Log("��� ������!");
        onGameComplete?.Invoke(false);
    }
}