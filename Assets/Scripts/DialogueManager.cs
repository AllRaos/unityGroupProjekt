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
    public Button exitButton;
    public Button passQuestButton;

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
        exitButton.onClick.AddListener(CloseDialogue);
        passQuestButton.onClick.AddListener(PassQuest);
    }

    public void StartDialogue(Quest quest, PlayerController playerRef, NPCInteraction npc)
    {
        currentQuest = quest;
        this.player = playerRef;
        this.currentNPC = npc;

        dialoguePanel.SetActive(true);
        questNameText.text = quest.questName;

        bool alreadyTaken = player.activeQuests.Exists(q => q.questName == quest.questName && q.status != QuestStatus.Finished);

        if (npc.isFinalNPC && !AllOtherQuestsCompleted(player, quest))
        {
            questDescriptionText.text = "��������� �� ��������� ��������, ��� �������� ��.";
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            passQuestButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
        }
        else if (quest.status == QuestStatus.Completed)
        {
            questDescriptionText.text = "�� �������� ����� ��������! ������ ���� �����.";
            passQuestButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
        }
        else if (quest.status == QuestStatus.Finished)
        {
            questDescriptionText.text = "��� ����� ��� ���������!";
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            passQuestButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
        }
        else if (alreadyTaken)
        {
            questDescriptionText.text = "��������� ������� ��������, ��� �������� ����.";
            acceptButton.gameObject.SetActive(false);
            declineButton.gameObject.SetActive(false);
            passQuestButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(true);
        }
        else
        {
            questDescriptionText.text = quest.questDescription;
            acceptButton.gameObject.SetActive(true);
            declineButton.gameObject.SetActive(true);
            passQuestButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
        }
    }

    void AcceptQuest()
    {
        if (!player.activeQuests.Exists(q => q.questName == currentQuest.questName))
        {
            currentQuest.status = QuestStatus.Active;
            player.activeQuests.Add(currentQuest);

            QuestLogUI logUI = FindAnyObjectByType<QuestLogUI>();
            if (logUI != null) logUI.RefreshUI();
        }

        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }

    void DeclineQuest()
    {
        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }

    void PassQuest()
    {
        player.TryCompleteQuest(currentNPC);
        dialoguePanel.SetActive(false);
        player.EndDialogue();
    }

    private bool AllOtherQuestsCompleted(PlayerController player, Quest finalQuest)
    {
        int finishedCount = 0;

        foreach (var quest in player.finishedQuests)
        {
            if (quest != finalQuest)
            {
                finishedCount++;
            }
        }

        return finishedCount >= 3;
    }

}