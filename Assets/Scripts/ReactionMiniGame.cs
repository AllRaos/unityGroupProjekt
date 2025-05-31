using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static PlayerController;

public class ReactionMiniGame : MonoBehaviour, IMiniGame
{
    public GameObject tileBackgroundPrefab; // Префаб білої плитки (Image)
    public GameObject fishButtonPrefab;     // Префаб рибки (Button)
    public Transform gridParent;            // Батьківський об’єкт для сітки
    public int gridSize = 5;

    private float fishLifetime;
    private int fishToCatch;
    private int caughtFish = 0;
    private int failCount = 0;

    private bool gridGenerated = false;
    private GameObject currentFishButton;
    private List<Transform> gridTiles = new List<Transform>();
    private System.Action<bool> onComplete;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        gameObject.SetActive(true);
        onComplete = onGameComplete;
        caughtFish = 0;
        failCount = 0;

        // Складність
        switch (size)
        {
            case FishSize.Large:
                fishLifetime = 1f;
                fishToCatch = 5;
                break;
            case FishSize.Medium:
                fishLifetime = 1.5f;
                fishToCatch = 7;
                break;
            case FishSize.Small:
                fishLifetime = 2f;
                fishToCatch = 10;
                break;
        }

        if (!gridGenerated)
            GenerateGrid();

        StartCoroutine(SpawnFishRoutine());
    }

    private void GenerateGrid()
    {
        gridTiles.Clear();

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject bgTile = Instantiate(tileBackgroundPrefab, gridParent);
            gridTiles.Add(bgTile.transform);
        }

        gridGenerated = true;
    }

    private IEnumerator SpawnFishRoutine()
    {
        yield return new WaitForSeconds(0.3f); // невелика пауза

        int randomIndex = Random.Range(0, gridTiles.Count);
        Transform targetTile = gridTiles[randomIndex];

        // Створення кнопки з рибкою
        currentFishButton = Instantiate(fishButtonPrefab, targetTile);
        currentFishButton.GetComponent<Button>().onClick.AddListener(() => OnFishClicked());

        yield return new WaitForSeconds(fishLifetime);

        // Якщо гравець не натиснув
        if (currentFishButton != null)
        {
            Destroy(currentFishButton);
            failCount++;
            if (failCount >= 2)
                FinishGame(false);
            else
                StartCoroutine(SpawnFishRoutine());
        }
    }

    private void OnFishClicked()
    {
        if (currentFishButton == null) return;

        Destroy(currentFishButton);
        caughtFish++;

        if (caughtFish >= fishToCatch)
            FinishGame(true);
        else
            StartCoroutine(WaitBeforeNextSpawn());
    }

    private IEnumerator WaitBeforeNextSpawn()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnFishRoutine());
    }

    private void FinishGame(bool success)
    {
        if (currentFishButton != null)
            Destroy(currentFishButton);

        gameObject.SetActive(false);
        Debug.Log(success ? "Гру завершено успішно!" : "Програш.");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();
    }
}
