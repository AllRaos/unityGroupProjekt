using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Quest quest;
    public GameObject interactionPromptPrefab;

    public bool isFinalNPC = false;

    private GameObject promptInstance;
    private bool playerInRange = false;

    void Start()
    {
        if (interactionPromptPrefab != null)
        {
            promptInstance = Instantiate(interactionPromptPrefab, transform);
            promptInstance.transform.localPosition = new Vector3(0, 0.3f, 0);
            promptInstance.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptInstance != null)
                promptInstance.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptInstance != null)
                promptInstance.SetActive(false);
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
    public bool TryGiveReward()
    {
        Quest playerQuest = PlayerController.Instance.activeQuests
            .Find(q => q.questName == quest.questName && (q.status == QuestStatus.Active || q.status == QuestStatus.Completed));

        if (playerQuest != null)
        {
            if (playerQuest.CheckIfQuestCompleted(PlayerController.Instance.inventory))
            {
                Item itemToRemove = new Item(
                    playerQuest.requiredFishName,
                    "риба",
                    playerQuest.requiredFishSize,
                    playerQuest.requiredFishCount
                );

                PlayerController.Instance.inventory.RemoveItem(itemToRemove);

                playerQuest.status = QuestStatus.Completed;
                Debug.Log(" вест виконано: " + playerQuest.questName);
                return true;
            }
        }
        return false;
    }
}