using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;

    private NPCInteraction currentNPC;
    private bool isDialogueActive = false;

    public List<Quest> activeQuests = new List<Quest>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            movement = Vector2.zero;
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
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = null;
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        rb.linearVelocity = Vector2.zero;

        Quest quest = currentNPC.GetQuest();
        if (!activeQuests.Exists(q => q.questName == quest.questName))
        {
            activeQuests.Add(quest);
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
            Debug.Log("����� ���������: " + quest.questName);
        }
    }
}
