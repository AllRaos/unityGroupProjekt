using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public List<Item> items = new List<Item>();


    public void AddItem(Item item)
    {

        Item existingItem = items.Find(i => i.name == item.name && i.type == item.type && i.size == item.size);
        if (existingItem != null)
        {

            existingItem.quantity += item.quantity;
        }
        else
        {

            items.Add(item);
        }
    }


    public void RemoveItem(Item item)
    {
        Item existingItem = items.Find(i => i.name == item.name && i.type == item.type && i.size == item.size);
        if (existingItem != null)
        {
            existingItem.quantity -= item.quantity;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
        }
    }


    public bool HasItem(Item item)
    {
        Item existingItem = items.Find(i => i.name == item.name && i.type == item.type && i.size == item.size);
        return existingItem != null && existingItem.quantity >= item.quantity;
    }
}