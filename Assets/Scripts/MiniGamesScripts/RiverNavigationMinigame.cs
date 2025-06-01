using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static PlayerController;

public class RiverNavigationMinigame : MonoBehaviour, IMiniGame
{
    public Canvas canvas;
    private RectTransform fish;
    private List<RectTransform> obstacles = new List<RectTransform>();
    private float scrollSpeed = 200f;
    private float fishSpeed = 200f;
    private float health = 100f;
    private float finishLineX = 800f;
    private float spawnInterval = 1f;
    private float timeSinceLastSpawn = 0f;
    private bool isPlaying = false;
    private Action<bool> onComplete;
    private FishType fishType;
    private FishSize fishSize;
    private Image healthBar;



    public void StartGame(FishType type, FishSize size, Action<bool> callback)
    {
        if (fish != null)
            Destroy(fish.gameObject);
        if (healthBar != null)
            Destroy(healthBar.gameObject);
        gameObject.SetActive(true);
        fishType = type;
        fishSize = size;
        onComplete = callback;
        isPlaying = true;
        health = 100f;
        fish = new GameObject("Fish").AddComponent<RectTransform>();
        healthBar = new GameObject("HealthBar").AddComponent<Image>();
        fish.anchoredPosition = new Vector2(-300, 0);
        healthBar.rectTransform.sizeDelta = new Vector2(200, 20);
        ClearObstacles();

        fish.SetParent(canvas.transform, false);
        fish.sizeDelta = new Vector2(50, 50);
        fish.anchoredPosition = new Vector2(-300, 0);
        Image fishImage = fish.gameObject.AddComponent<Image>();
        fishImage.color = Color.blue;

        healthBar.rectTransform.SetParent(canvas.transform, false);
        healthBar.rectTransform.sizeDelta = new Vector2(200, 20);
        healthBar.rectTransform.anchoredPosition = new Vector2(0, 200);
        healthBar.color = Color.green;

        switch (size)
        {
            case FishSize.Small:
                scrollSpeed = 150f;
                fishSpeed = 250f;
                spawnInterval = 1.2f;
                break;
            case FishSize.Medium:
                scrollSpeed = 200f;
                fishSpeed = 200f;
                spawnInterval = 1f;
                break;
            case FishSize.Large:
                scrollSpeed = 250f;
                fishSpeed = 150f;
                spawnInterval = 0.8f;
                break;
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        float input = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
        float newY = fish.anchoredPosition.y + input * fishSpeed * Time.deltaTime;
        newY = Mathf.Clamp(newY, -200, 200);

        float newX = fish.anchoredPosition.x + scrollSpeed * Time.deltaTime;

        fish.anchoredPosition = new Vector2(newX, newY);

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnObstacle();
            timeSinceLastSpawn = 0f;
        }

        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            RectTransform obstacle = obstacles[i];
            obstacle.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);
            if (obstacle.anchoredPosition.x < -400)
            {
                Destroy(obstacle.gameObject);
                obstacles.RemoveAt(i);
            }
            else if (IsColliding(fish, obstacle))
            {
                health -= 50f;
                healthBar.rectTransform.sizeDelta = new Vector2(health * 2, 20);
                Destroy(obstacle.gameObject);
                obstacles.RemoveAt(i);
            }
        }

        if (fish.anchoredPosition.x >= finishLineX)
        {
            EndGame(true);
        }
        else if (health <= 0)
        {
            EndGame(false);
        }
    }

    private void SpawnObstacle()
    {
        RectTransform obstacle = new GameObject("Obstacle").AddComponent<RectTransform>();
        obstacle.SetParent(canvas.transform, false);
        obstacle.sizeDelta = new Vector2(50, 50);
        obstacle.anchoredPosition = new Vector2(400, UnityEngine.Random.Range(-200, 200));
        Image obstacleImage = obstacle.gameObject.AddComponent<Image>();
        obstacleImage.color = Color.red;
        obstacles.Add(obstacle);
    }

    private bool IsColliding(RectTransform a, RectTransform b)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(a, b.position) ||
               RectTransformUtility.RectangleContainsScreenPoint(b, a.position);
    }

    private void ClearObstacles()
    {
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        obstacles.Clear();
    }

    private void EndGame(bool success)
    {
        isPlaying = false;
        gameObject.SetActive(false);
        ClearObstacles();
        Debug.Log(success ? "Успішне завершення!" : "Ви програли!");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();
    }
}