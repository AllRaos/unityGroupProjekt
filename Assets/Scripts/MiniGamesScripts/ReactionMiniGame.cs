using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static PlayerController;

public class ReactionMiniGame : MonoBehaviour, IMiniGame
{
    public GameObject tileBackgroundPrefab;
    public GameObject fishButtonPrefab;
    public Transform gridParent;
    public int gridSize = 5;

    private float fishLifetime;
    private int fishToCatch;
    private int caughtFish = 0;
    private int failCount = 0;

    private bool gridGenerated = false;
    private Coroutine currentRoutine;

    private GameObject currentFishButton;
    private List<Transform> gridTiles = new List<Transform>();
    private System.Action<bool> onComplete;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        gameObject.SetActive(true);
        onComplete = onGameComplete;
        caughtFish = 0;
        failCount = 0;

        switch (size)
        {
            case FishSize.Large:
                fishLifetime = 0.7f;
                fishToCatch = 10;
                break;
            case FishSize.Medium:
                fishLifetime = 1f;
                fishToCatch = 7;
                break;
            case FishSize.Small:
                fishLifetime = 1.5f;
                fishToCatch =5;
                break;
        }

        if (!gridGenerated)
            GenerateGrid();

        StartNextFish();
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

    private void StartNextFish()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SpawnFishRoutine());
    }

    private IEnumerator SpawnFishRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        int randomIndex = Random.Range(0, gridTiles.Count);
        Transform targetTile = gridTiles[randomIndex];

        currentFishButton = Instantiate(fishButtonPrefab, targetTile);
        currentFishButton.GetComponent<Button>().onClick.AddListener(OnFishClicked);

        float timer = 0f;
        while (timer < fishLifetime)
        {
            if (currentFishButton == null)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        if (currentFishButton != null)
        {
            Destroy(currentFishButton);
            currentFishButton = null;
            failCount++;

            if (failCount >= 2)
                FinishGame(false);
            else
                StartNextFish();
        }
    }

    private void OnFishClicked()
    {
        if (currentFishButton == null) return;

        Destroy(currentFishButton);
        currentFishButton = null;

        caughtFish++;

        if (caughtFish >= fishToCatch)
            FinishGame(true);
        else
            StartCoroutine(WaitBeforeNextSpawn());
    }

    private IEnumerator WaitBeforeNextSpawn()
    {
        yield return new WaitForSeconds(1f);
        StartNextFish();
    }

    private void FinishGame(bool success)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (currentFishButton != null)
            Destroy(currentFishButton);

        currentFishButton = null;

        gameObject.SetActive(false);
        Debug.Log(success ? "Гру завершено успішно!" : "Програш.");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();
    }
}
