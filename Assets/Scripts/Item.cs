[System.Serializable]
public class Item
{
    public string name;
    public string type;
    public string size;
    public int quantity;

    public Item(string name, string type, string size, int quantity)
    {
        this.name = name;
        this.type = type;
        this.size = size;
        this.quantity = quantity;
    }
}