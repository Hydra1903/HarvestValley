using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int rows = 4;
    public int columns = 8;

    public InventorySlot[,] slots;
    void Awake()
    {
        slots = new InventorySlot[rows, columns];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                slots[r, c] = new InventorySlot();
            }
        }
    }
    public bool AddItem(ItemData data, int amount)
    {       
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (!slot.IsEmpty && slot.item.itemData == data && !slot.item.IsFull)
                {
                    int canAdd = Mathf.Min(amount, data.maxStack - slot.item.quantity);
                    slot.item.quantity += canAdd;
                    amount -= canAdd;
                    if (amount <= 0) return true;
                }
            }
        }// THÊM VÀO CÙNG KIỂU ITEM

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (slot.IsEmpty)
                {
                    int add = Mathf.Min(amount, data.maxStack);
                    slot.item = new InventoryItem(data, add);
                    amount -= add;
                    if (amount <= 0) return true;
                }
            }
        }// TẠO Ô MỚI THÊM VÀO

        return false;
    }
    public bool AddItem2(ItemData data, int amount)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (slot.IsEmpty)
                {
                    int add = Mathf.Min(amount, data.maxStack);
                    slot.item = new InventoryItem(data, add);
                    amount -= add;
                    if (amount <= 0) return true;
                }
            }
        }// TẠO Ô MỚI THÊM VÀO

        return false;
    }

    public void ShowArray()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Debug.Log(slots[r,c]);
            }
        }
    }
}
// Nơi lưu trữ tất cả thông tin về vật phẩm có trong túi đồ, sử dụng mảng 2 chiều để lưu trữ, mỗi vị trí trong mảng là 1 ô đồ
// Ban đầu vào sẽ khởi tạo cái mảng 2 chiều rỗng
// Hàm thêm item vào mảng 2 chiều tức là thêm item vào túi đồ người chơi, nếu hàm trả về true thì thêm item thành công, nếu hàm trả về false thì bị đầy hoặc lí do khác
