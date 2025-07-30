using UnityEngine;
using TMPro;

public class Barn : MonoBehaviour
{
    public int rows = 5;
    public int columns = 7;
    public InventorySlot[,] slots;
    public int limitCapacity = 1000;
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
        Debug.Log("them");
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
        }// THÊM VÀO CÙNG DATA

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
        Debug.Log("them");
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
}
