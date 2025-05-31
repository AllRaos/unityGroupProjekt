using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static PlayerController;

public class SpeedMiniGame : MonoBehaviour, IMiniGame
{
    public RectTransform sliderBlock;
    public RectTransform hitZone;
    public RectTransform barArea;
    public float moveSpeed = 800f;
    public float hitCooldown = 1f;
    public float totalDuration = 20f;

    private int successCount;
    private int failCount;
    private bool isMovingRight = true;
    private float timer;
    private bool canHit = true;

    private System.Action<bool> onComplete;
    private Coroutine gameRoutine;

    public void StartGame(FishType type, FishSize size, System.Action<bool> onGameComplete)
    {
        gameObject.SetActive(true);

        successCount = 0;
        failCount = 0;
        timer = 0f;
        canHit = true;
        onComplete = onGameComplete;

        moveSpeed = size == FishSize.Small ? 800f : size == FishSize.Medium ? 900f : 1000f;
        totalDuration = size == FishSize.Small ? 20f : size == FishSize.Medium ? 15f : 10f;
        float baseWidth = size == FishSize.Small ? 600f : size == FishSize.Medium ? 500f : 400f;
        sliderBlock.sizeDelta = new Vector2(baseWidth, sliderBlock.sizeDelta.y);

        sliderBlock.anchoredPosition = Vector2.zero;

        hitZone.anchoredPosition = new Vector2(-barArea.rect.width / 2f, 0);

        if (gameRoutine != null) StopCoroutine(gameRoutine);
        gameRoutine = StartCoroutine(RunGame());
    }

    private IEnumerator RunGame()
    {
        while (timer < totalDuration && successCount < 3 && failCount < 2)
        {
            timer += Time.deltaTime;

            float move = moveSpeed * Time.deltaTime * (isMovingRight ? 1 : -1);
            hitZone.anchoredPosition += new Vector2(move, 0);

            if (hitZone.anchoredPosition.x > barArea.rect.width / 2f)
            {
                isMovingRight = false;
                hitZone.anchoredPosition = new Vector2(barArea.rect.width / 2f, 0);
            }
            else if (hitZone.anchoredPosition.x < -barArea.rect.width / 2f)
            {
                isMovingRight = true;
                hitZone.anchoredPosition = new Vector2(-barArea.rect.width / 2f, 0);
            }

            if (Input.GetKeyDown(KeyCode.F) && canHit)
            {
                canHit = false;
                StartCoroutine(HitCooldown());

                if (IsInsideHitZone())
                {
                    successCount++;
                    Debug.Log("Влучив!");
                    sliderBlock.sizeDelta = new Vector2(sliderBlock.sizeDelta.x * 0.5f, sliderBlock.sizeDelta.y);
                }
                else
                {
                    failCount++;
                    Debug.Log("Промах!");
                }
            }

            yield return null;
        }

        FinishGame(successCount >= 3);
    }

    private IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }

    private bool IsInsideHitZone()
    {
        Vector3 blockWorldPos = sliderBlock.position;
        float blockWidth = sliderBlock.rect.width;

        Vector3 hitWorldPos = hitZone.position;
        float hitWidth = hitZone.rect.width;

        float blockMinX = blockWorldPos.x - blockWidth / 2f;
        float blockMaxX = blockWorldPos.x + blockWidth / 2f;

        float hitMinX = hitWorldPos.x - hitWidth / 2f;
        float hitMaxX = hitWorldPos.x + hitWidth / 2f;

        return hitMaxX >= blockMinX && hitMinX <= blockMaxX;
    }

    private void FinishGame(bool success)
    {
        gameObject.SetActive(false);
        Debug.Log(success ? "Успішне завершення!" : "Ви програли!");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();
    }
}
