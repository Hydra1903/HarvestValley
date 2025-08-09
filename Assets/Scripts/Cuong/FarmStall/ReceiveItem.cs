using UnityEngine;

public class ReceiveItem : MonoBehaviour
{
    public InventorySlot[] slots;
    public int locationDataItem;
    public Inventory inventory;
    void Awake()
    {
        slots = new InventorySlot[2];
        slots[0] = new InventorySlot();
        slots[1] = new InventorySlot();
    }
    public void DestroyDataItem()
    {
        slots[0] = new InventorySlot();
        slots[1] = new InventorySlot();
    }
    public void ReturnItem()
    {
        if (slots[0].item != null)
        {
            inventory.AddItem(slots[0].item.itemData, slots[0].item.quantity);
        }
        if (slots[1].item != null)
        {
            inventory.AddItem(slots[1].item.itemData, slots[1].item.quantity);
        }
        DestroyDataItem();
    }
}
