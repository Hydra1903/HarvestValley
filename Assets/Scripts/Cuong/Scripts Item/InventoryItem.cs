[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData data, int amount)
    {
        itemData = data;
        quantity = amount;
    }

    public bool IsFull => quantity >= itemData.maxStack;
}
// Là thông tin dữ liệu của item trong 1 ô bao gồm dữ liệu gốc của item và số lượng item mà ô đó đang chứa, điều kiện kiểm tra xem ô đó có bị đầy vật phẩm chưa

