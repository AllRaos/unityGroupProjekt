using UnityEngine;
using UnityEngine.UI;
using System;
using static PlayerController;

public class TensionZoneMinigame : MonoBehaviour, IMiniGame
{
    public Canvas canvas;
    private Slider tensionSlider;
    private Image greenZone;
    private Image backgroundImage;
    private Image redZoneOverlay;

    private float cursorSpeed = 0.5f;
    private float greenZoneMin = 0.4f;
    private float greenZoneMax = 0.6f;
    private float timeInZone = 0f;
    private float timeOutsideZone = 0f;
    private float maxTimeInZone = 5f;
    private float maxTimeOutsideZone = 2f;
    private bool isPlaying = false;
    private Action<bool> onComplete;
    private FishType fishType;
    private FishSize fishSize;



    public void StartGame(FishType type, FishSize size, Action<bool> callback)
    {
        if (backgroundImage != null)
            Destroy(backgroundImage.gameObject);
        gameObject.SetActive(true);
        fishType = type;
        fishSize = size;
        onComplete = callback;
        isPlaying = true;
        timeInZone = 0f;
        timeOutsideZone = 0f;
        tensionSlider = GetComponentInChildren<Slider>();
        tensionSlider.value = 0.5f;
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);

        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = Color.gray;

        RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;


        bgObj.transform.SetAsFirstSibling();

        // Налаштування слайдера
        tensionSlider.minValue = 0f;
        tensionSlider.maxValue = 1f;
        tensionSlider.value = 0.5f;
        RectTransform sliderRect = tensionSlider.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.25f, 0.45f);
        sliderRect.anchorMax = new Vector2(0.75f, 0.55f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        // Налаштування зеленої зони
        greenZone = tensionSlider.GetComponentInChildren<Image>();
        RectTransform greenZoneRect = greenZone.GetComponent<RectTransform>();
        greenZoneRect.anchorMin = new Vector2(greenZoneMin, 0);
        greenZoneRect.anchorMax = new Vector2(greenZoneMax, 1);
        greenZoneRect.offsetMin = Vector2.zero;
        greenZoneRect.offsetMax = Vector2.zero;
        greenZone.color = Color.green;

        switch (size)
        {
            case FishSize.Small:
                cursorSpeed = 0.3f;
                maxTimeInZone = 4f;
                maxTimeOutsideZone = 2.5f;
                break;
            case FishSize.Medium:
                cursorSpeed = 0.5f;
                maxTimeInZone = 5f;
                maxTimeOutsideZone = 2f;
                break;
            case FishSize.Large:
                cursorSpeed = 0.7f;
                maxTimeInZone = 6f;
                maxTimeOutsideZone = 1.5f;
                break;
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        float input = Input.GetKey(KeyCode.F) ? cursorSpeed : -cursorSpeed;
        tensionSlider.value += input * Time.deltaTime;
        tensionSlider.value = Mathf.Clamp01(tensionSlider.value);

        bool inZone = tensionSlider.value >= greenZoneMin && tensionSlider.value <= greenZoneMax;
        if (inZone)
        {
            timeInZone += Time.deltaTime;
            timeOutsideZone = 0f;
        }
        else
        {
            timeOutsideZone += Time.deltaTime;
        }

        if (timeInZone >= maxTimeInZone)
        {
            EndGame(true);
        }
        else if (timeOutsideZone >= maxTimeOutsideZone)
        {
            EndGame(false);
        }
    }

    private void EndGame(bool success)
    {
        isPlaying = false;
        gameObject.SetActive(false);
        Debug.Log(success ? "Успішне завершення!" : "Ви програли!");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();
    }
}