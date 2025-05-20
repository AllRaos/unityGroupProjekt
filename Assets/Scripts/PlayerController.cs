using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    public GameObject InteractionPrompt;
    private NPCInteraction currentNPC; // Посилання на NPC, з яким можна взаємодіяти
    private bool isDialogueActive = false; // Чи відкрите діалогове вікно
    public List<Quest> activeQuests = new List<Quest>(); // Список активних квестів
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InteractionPrompt.SetActive(false);
    }

    void Update()
    {
        if (!isDialogueActive)
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
            movement.x = 0;
            movement.y = 0;
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
            if (currentNPC != null)
            {
                InteractionPrompt.SetActive(true);
                Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(other.transform.position);
                playerScreenPos.y += 150f;
                InteractionPrompt.transform.position = playerScreenPos;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            InteractionPrompt.SetActive(false);
            currentNPC = null;
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        rb.linearVelocity = Vector2.zero;
        InteractionPrompt.SetActive(false);

        Quest quest = currentNPC.GetQuest();
        if (!activeQuests.Exists(q => q.questName == quest.questName))
        {
            activeQuests.Add(quest); // Додаємо квест, якщо його ще немає
        }
        DialogueManager.Instance.StartDialogue(quest, this, currentNPC);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
    }

    public void TryCompleteQuest(NPCInteraction npc)
    {
        Quest quest = activeQuests.Find(q => q.questName == npc.quest.questName && q.status == QuestStatus.Completed);
        if (quest != null)
        {
            quest.status = QuestStatus.Finished;
            Debug.Log("Квест завершено: " + quest.questName);
            // Тут можна додати нагороду
        }
    }
}
