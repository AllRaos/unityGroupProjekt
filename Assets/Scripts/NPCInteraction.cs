using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Quest quest; // ��������� ����� ��� ����� NPC
    private bool playerInRange = false;
    public GameObject player;

    void Start()
    {
        // ����������� ������ � ��������� Unity ��� ���
        // ���������, ��� ������� NPC:
        // quest = new Quest("������� 3 ������", "������� �� �������� 3 ����-�� ������", new CatchAnyFishCondition(3));
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