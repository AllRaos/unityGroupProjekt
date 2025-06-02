using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    public Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    private NPCInteraction currentNPC;
    private bool isDialogueActive = false;

    public List<Quest> activeQuests = new List<Quest>();

    public Inventory inventory;
    public GameObject inventoryPanel;
    public GameObject backgroundShadow;
    private bool isInventoryOpen = false;

    public List<Quest> finishedQuests = new List<Quest>();

    public enum FishType { GoldFish, Globefish, GreenAngelfish, Smartfish, Shark }
    public enum FishSize { Small, Medium, Large }

    public bool isFishing = false;
    public float fishingTimer = 0f;
    public FishType currentFishType;
    public FishSize currentFishSize;
    public bool isNearWater = false;

    public bool isMiniGameActive = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventory = new Inventory();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isMiniGameActive)
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
            backgroundShadow.SetActive(isInventoryOpen);
            if (isInventoryOpen)
            {
                InventoryUI.instance.UpdateInventoryUI(inventory.items);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isFishing && !isMiniGameActive && IsNearWater())
        {
            StartFishing();
        }

        if (isFishing)
        {
            fishingTimer += Time.deltaTime;
            if (fishingTimer >= Random.Range(2f, 4f))
            {
                CatchFish();
            }
        }

        if (!isDialogueActive && !isMiniGameActive && !isFishing)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (currentNPC != null && currentNPC.CanInteract() && Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }

            if (movement != Vector2.zero)
            {
                animator.SetTrigger("Walk");

                if (movement.x != 0)
                    spriteRenderer.flipX = movement.x < 0;
            }
            else
            {
                animator.ResetTrigger("Walk");
            }
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private bool IsNearWater()
    {
        return isNearWater;
    }

    private void StartFishing()
    {
        isFishing = true;
        fishingTimer = 0f;
        animator.SetTrigger("Fish");

        MusicManager musicManager = FindAnyObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.PlayFishingMusic();
        }
    }

    private void CatchFish()
    {
        if (isMiniGameActive) return;
        isFishing = false;
        if (activeQuests.Any(q => q.questName == "Квест №4"))
        {
            currentFishType = FishType.Shark;
            currentFishSize = FishSize.Large;
        }
        else
        {
            currentFishType = (FishType)Random.Range(0, 4);
            currentFishSize = (FishSize)Random.Range(0, 3);
        }
        StartFishingGame(currentFishType, currentFishSize);

    }

    private void StartFishingGame(FishType type, FishSize size)
    {
        if (isMiniGameActive) return;
        isMiniGameActive = true;

        switch (type)
        {
            case FishType.Globefish:
                Debug.Log($"Запуск міні-гри для рибки Надуваки, складність: {size}");
                GetComponent<FishingGame>().StartMiniGame(type, size);
                break;
            case FishType.GreenAngelfish:
                Debug.Log($"Запуск міні-гри для Зеленої рибки, складність: {size}");
                GetComponent<FishingGame>().StartMiniGame(type, size);
                break;
            case FishType.Smartfish:
                Debug.Log($"Запуск міні-гри для риби Розумник, складність: {size}");
                GetComponent<FishingGame>().StartMiniGame(type, size);
                break;
            case FishType.GoldFish:
                Debug.Log($"Запуск міні-гри для Золотої рибки, складність: {size}");
                GetComponent<FishingGame>().StartMiniGame(type, size);
                break;
            case FishType.Shark:
                Debug.Log($"Запуск міні-гри для Акули, складність: {size}");
                GetComponent<FishingGame>().StartMiniGame(type, size);
                break;
        }
    }

    public void OnMiniGameSuccess()
    {
        Item fish = new Item(currentFishType.ToString(), "риба", currentFishSize.ToString(), 1);
        inventory.AddItem(fish);
        Debug.Log($"Риба {currentFishType} ({currentFishSize}) додана до інвентарю!");

        CheckAllQuestsForCompletion();
    }

    public void EndMiniGame()
    {
        isMiniGameActive = false;

        MusicManager musicManager = FindAnyObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.PlayMainMusic();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPCInteraction>();
        }
        else if (other.CompareTag("Water"))
        {
            isNearWater = true;
            Debug.Log("You can start fishing");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = null;
        }
        else if (other.CompareTag("Water"))
        {
            isNearWater = false;
            Debug.Log("You can not start fishing");
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        rb.linearVelocity = Vector2.zero;

        Quest quest = currentNPC.GetQuest();
        DialogueManager.Instance.StartDialogue(quest, this, currentNPC);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
    }

    public void TryCompleteQuest(NPCInteraction npc)
    {
        if (npc.TryGiveReward())
        {
            Quest quest = activeQuests.Find(q => q.questName == npc.quest.questName && q.status == QuestStatus.Completed);
            if (quest != null)
            {
                quest.status = QuestStatus.Finished;
                Debug.Log("Квест здано: " + quest.questName);

                activeQuests.Remove(quest);
                if (!finishedQuests.Contains(quest))
                {
                    finishedQuests.Add(quest);
                }
                QuestLogUI logUI = FindAnyObjectByType<QuestLogUI>();
                if (logUI != null) logUI.RefreshUI();
            }
        }
        else
        {
            Debug.Log("Потрібні предмети відсутні або квест вже виконано.");
        }
    }

    private void CheckAllQuestsForCompletion()
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.status == QuestStatus.Active && quest.CheckIfQuestCompleted(inventory))
            {
                quest.status = QuestStatus.Completed;
                Debug.Log($"Квест '{quest.questName}' автоматично позначено як виконаний!");
            }
        }
    }

}
