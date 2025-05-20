using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Quest quest; // Унікальний квест для цього NPC
    private bool playerInRange = false;
    public GameObject player;

    void Start()
    {
        // Ініціалізація квестів у інспекторі Unity або тут
        // Наприклад, для першого NPC:
        // quest = new Quest("Зловити 3 рибини", "Зловити та принести 3 будь-які рибини", new CatchAnyFishCondition(3));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    public bool CanInteract()
    {
        return playerInRange;
    }

    public Quest GetQuest()
    {
        return quest;
    }
}