using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public Transform itemsParent;
    public GameObject itemPrefab;
    public Sprite[] fishSprites;

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

            Image fishImage = itemUI.transform.Find("FishImage").GetComponent<Image>();
            fishImage.sprite = GetFishSprite(item.name);

            TextMeshProUGUI text = itemUI.transform.Find("InfoText").GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{item.name} ({item.size}) x{item.quantity}";
            }
        }
    }

    private Sprite GetFishSprite(string fishName)
    {
        foreach (Sprite s in fishSprites)
        {
            if (s.name == fishName)
                return s;
        }

        Debug.LogWarning($"Спрайт для риби '{fishName}' не знайдено");
        return null;
    }

}
