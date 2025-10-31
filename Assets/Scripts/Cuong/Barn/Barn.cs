using UnityEngine;
using TMPro;

public class Barn : MonoBehaviour
{
    public static Barn Instance;
    public int rows = 5;
    public int columns = 7;
    public InventorySlot[,] slots;
    public int limitCapacity;
    public TextMeshProUGUI textlevelBarn;

    public ItemData[] saveItemData = new ItemData[35];
    public int[] saveQuantity = new int[35];
    public bool[] saveLocation = new bool[35];
    private void Start()
    {
        switch(Builder.Instance.currentlevelBarn)
        {
            case 1:
                limitCapacity = 500;
                textlevelBarn.text = "Cấp 1";
                break;
            case 2:
                limitCapacity = 1000;
                textlevelBarn.text = "Cấp 2";
                break;
            case 3:
                limitCapacity = 2500;
                textlevelBarn.text = "Cấp 3";
                break;
        }
    }
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        slots = new InventorySlot[rows, columns];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                slots[r, c] = new InventorySlot();
            }
        }
    }
    public void SaveItem()
    {
        saveItemData = new ItemData[35];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (slots[r, c].item != null)
                {
                    saveItemData[r * 7 + c] = slots[r, c].item.itemData;
                    saveQuantity[r * 7 + c] = slots[r, c].item.quantity;
                }
            }
        }
    }
    public void LoadItem()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (saveItemData[r * 7 + c] != null)
                {
                    AddItem3(saveItemData[r * 7 + c], saveQuantity[r * 7 + c]);
                }
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
    public bool AddItem3(ItemData data, int amount)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var slot = slots[r, c];
                if (slot.IsEmpty && saveLocation[r * 7 + c])
                {
                    int add = Mathf.Min(amount, data.maxStack);
                    slot.item = new InventoryItem(data, add);
                    amount -= add;
                    if (amount <= 0)
                    {
                        return true;
                    }
                }
            }
        }// TẠO Ô MỚI THÊM VÀO

        return false;
    }
}
