using UnityEngine;
using static PlayerController;
using System.Collections;
public class ReactionMiniGame : MonoBehaviour, IMiniGame
{
    private float timer;
    private float targetTime;
    private bool isRunning;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        // ������������ ��������� ������� �� ������ ����
        float difficultyMultiplier = size == FishSize.Small ? 1.5f : size == FishSize.Medium ? 1.0f : 0.5f;
        targetTime = Random.Range(2f, 5f) * difficultyMultiplier;
        timer = 0f;
        isRunning = true;

        Debug.Log($"��������� ��-��� ��������. �������� ����� � �������� ������! ���: {targetTime:F2} ���");

        // ��������� �������� ��� ���
        StartCoroutine(RunGame(onGameComplete));
    }

    public IEnumerator RunGame(System.Action<bool> onGameComplete)
    {
        while (timer < targetTime + 1f)
        {
            timer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool success = Mathf.Abs(timer - targetTime) < 0.5f; // ���� ����� �0.5 ���
                Debug.Log(success ? "����! �� ��������� ������!" : "������! ������� ���� ��� ����.");
                onGameComplete?.Invoke(success);
                yield break;
            }
            yield return null;
        }

        Debug.Log("��� ������!");
        onGameComplete?.Invoke(false);
    }
}