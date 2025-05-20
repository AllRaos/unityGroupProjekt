using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Transform itemsParent;
    public GameObject itemPrefab;

    void Awake()
    {
        instance = this;
    }

    public void UpdateInventoryUI(List<Item> items)
    {

        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }


        foreach (Item item in items)
        {
            GameObject itemUI = Instantiate(itemPrefab, itemsParent);
            TextMeshProUGUI text = itemUI.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{item.name} x{item.quantity}";
            }
        }
    }
}
