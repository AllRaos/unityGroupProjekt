using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialoguePanel;
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    public Button acceptButton;
    public Button declineButton;
    private Quest currentQuest;
    private PlayerController player;
    private NPCInteraction currentNPC;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        acceptButton.onClick.AddListener(AcceptQuest);
        declineButton.onClick.AddListener(DeclineQuest);
    }

    public void StartDialogue(Quest quest, PlayerController playerRef, NPCInteraction npc)
    {
        currentQuest = quest;
        this.player = playerRef;
        this.currentNPC = npc;

        dialoguePanel.SetActive(true);
        questNameText.text = quest.questName;
        questDescriptionText.text = quest.questDescription;

        if (quest.status == QuestStatus.Active)
        {
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
        }
        else if (quest.status == QuestStatus.Completed)
        {
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            player.TryCompleteQuest(currentNPC);
            dialoguePanel.SetActive(false);
            player.EndDialogue();
        }
        else if (quest.status == QuestStatus.Finished)
        {
            questDescriptionText.text = "????? ??? ?????????!";
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
        }
    }

    void AcceptQuest()
    {
        Debug.Log("????? ????????: " + currentQuest.questName);
        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }

    void DeclineQuest()
    {
        Debug.Log("????? ????????");
        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }
}