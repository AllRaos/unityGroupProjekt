using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestLogUI : MonoBehaviour
{
    public GameObject questEntryPrefab;
    public Transform contentParent;
    public GameObject panel;
    public Button toggleButton;
    private bool isOpen = true;

    void Start()
    {
        toggleButton.onClick.AddListener(TogglePanel);
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        var quests = PlayerController.Instance.activeQuests;

        foreach (var quest in quests)
        {
            GameObject entry = Instantiate(questEntryPrefab, contentParent);
            TMP_Text titleText = entry.transform.Find("QuestTitle").GetComponent<TMP_Text>();
            TMP_Text descriptionText = entry.transform.Find("QuestDescription").GetComponent<TMP_Text>();

            titleText.text = quest.questName;
            descriptionText.text = quest.questDescription;
            descriptionText.gameObject.SetActive(false);

            Button button = entry.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                bool isActive = !descriptionText.gameObject.activeSelf;
                descriptionText.gameObject.SetActive(isActive);

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)entry.transform);
            });

        }
    }

    void TogglePanel()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);
    }
}
