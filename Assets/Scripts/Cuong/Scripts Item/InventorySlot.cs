[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;
    public bool IsEmpty => item == null;
}
// Thể hiện 1 ô dữ liệu trong mảng chiều của Inventory bao gồm 1 dữ liệu về item và 1 điều kiện kiểm tra item có rỗng không (tức là ô đó đang không có vật phẩm)