using UnityEngine;
using static PlayerController;
using System.Collections;

public class AccuracyMiniGame : MonoBehaviour, IMiniGame
{
    private Vector2 targetPosition;
    private float targetRadius;
    private bool isRunning;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        // ������������ ��������� ������� �� ������ ����
        targetRadius = size == FishSize.Small ? 0.2f : size == FishSize.Medium ? 0.5f : 0.8f;
        targetPosition = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        isRunning = true;

        Debug.Log($"������� ��-��� ��������. ������ � ����� ������ {targetRadius} �� ��� ({targetPosition.x:F2}, {targetPosition.y:F2})!");

        // ��������� �������� ��� ���
        StartCoroutine(RunGame(onGameComplete));
    }

    public IEnumerator RunGame(System.Action<bool> onGameComplete)
    {
        float timeLimit = 5f;
        float timer = 0f;

        while (timer < timeLimit)
        {
            timer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool success = Vector2.Distance(mousePos, targetPosition) <= targetRadius;
                Debug.Log(success ? "����! �� ������ � ����!" : "������!");
                onGameComplete?.Invoke(success);
                yield break;
            }
            yield return null;
        }

        Debug.Log("��� ������!");
        onGameComplete?.Invoke(false);
    }
}