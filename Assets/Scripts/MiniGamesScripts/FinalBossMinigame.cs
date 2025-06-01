using UnityEngine;
using UnityEngine.UI;
using System;
using static PlayerController;


public class FinalBossMinigame : MonoBehaviour, IMiniGame
{
    public RectTransform hookPointer;
    public RectTransform sharkObject;
    public Image attackIndicator;
    public Slider sharkStaminaBar;
    public Slider lineDurabilityBar;

    public float hookRotationSpeed = 180f;
    public float hookInertia = 0.1f;
    public float sharkAttackDistance = 200f;
    public float sharkAttackDuration = 0.5f;

    private float hookAngle = 0f;
    private float hookTargetAngle = 0f;
    private float attackAngle = 0f;
    private float timeToNextAttack = 0f;

    private float sharkStamina = 100f;
    private float lineDurability = 100f;

    private float attackCooldown = 2f;
    private float attackTolerance = 25f;

    private bool isPlaying = false;
    private Action<bool> onComplete;
    private FishType fishType;
    private FishSize fishSize;

    public void StartGame(FishType type, FishSize size, Action<bool> callback)
    {
        gameObject.SetActive(true);
        fishType = type;
        fishSize = size;
        onComplete = callback;
        isPlaying = true;
        sharkStamina = 100f;
        lineDurability = 100f;
        timeToNextAttack = 2f;
        hookAngle = 0f;
        hookTargetAngle = 0f;
        sharkObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

        HandleHookRotation();
        UpdateAttack();

        sharkStaminaBar.value = sharkStamina / 100f;
        lineDurabilityBar.value = lineDurability / 100f;

        if (sharkStamina <= 0f)
        {
            EndGame(true);
        }
        else if (lineDurability <= 0f)
        {
            EndGame(false);
        }
    }

    void HandleHookRotation()
    {
        if (Input.GetKey(KeyCode.A)) hookTargetAngle += hookRotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) hookTargetAngle -= hookRotationSpeed * Time.deltaTime;

        hookAngle = Mathf.LerpAngle(hookAngle, hookTargetAngle, hookInertia);
        hookPointer.localRotation = Quaternion.Euler(0, 0, hookAngle + 180f);
    }

    void UpdateAttack()
    {
        timeToNextAttack -= Time.deltaTime;

        if (timeToNextAttack <= 0f)
        {
            attackAngle = UnityEngine.Random.Range(0f, 360f);
            attackIndicator.transform.localRotation = Quaternion.Euler(0, 0, attackAngle + 180f);
            attackIndicator.gameObject.SetActive(true);

            StartCoroutine(SharkAttackSequence());
            timeToNextAttack = attackCooldown + UnityEngine.Random.Range(-0.5f, 0.5f);
        }
    }

    System.Collections.IEnumerator SharkAttackSequence()
    {
        yield return new WaitForSeconds(0.8f);
        attackIndicator.gameObject.SetActive(false);

        Vector2 startPos = CalculateSharkStartPosition(attackAngle);
        sharkObject.anchoredPosition = startPos;
        sharkObject.gameObject.SetActive(true);
        sharkObject.localRotation = Quaternion.Euler(0, 0, attackAngle);

        float elapsed = 0f;
        Vector2 targetPos = Vector2.zero;
        while (elapsed < sharkAttackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / sharkAttackDuration;
            sharkObject.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        ResolveAttack();
        sharkObject.gameObject.SetActive(false);
    }

    Vector2 CalculateSharkStartPosition(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Cos(rad) * sharkAttackDistance,
            Mathf.Sin(rad) * sharkAttackDistance
        );
    }

    void ResolveAttack()
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(hookAngle, attackAngle));

        if (diff <= attackTolerance)
        {
            sharkStamina -= 15f;
            Debug.Log("Парировано!");
        }
        else
        {
            lineDurability -= 20f;
            Debug.Log("Акула атакує!");
        }
    }

    void EndGame(bool success)
    {
        isPlaying = false;
        gameObject.SetActive(false);
        Debug.Log(success ? "Успішне завершення!" : "Ви програли, снасті порвані!");
        PlayerController.Instance.animator.SetTrigger("Hook");
        onComplete?.Invoke(success);
        PlayerController.Instance.EndMiniGame();

    }
}