using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform slotParent;
    public InventorySlot[] slots;
    public InventoryDragItem dragItem;

    [HideInInspector] public int dragSourceIndex = -1;

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
                slots[i].Set(inventory.items[i], i, this);
            else
                slots[i].Clear();
        }
    }
}

