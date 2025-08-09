using UnityEngine;

public class HotBar : MonoBehaviour
{
    public InventorySlot[] slots;
    public Inventory inventory;
    void Awake()
    {
        slots = new InventorySlot[8];
        for (int i = 0; i < 8; i++)
        {
            slots[i] = new InventorySlot();
        }
    }
    public void UpdateData()
    {
        for (int i = 0; i < 8; i++)
        {
            slots[i].item = inventory.slots[3, i].item;
        }
    }
    public void UseAndRemoveItem(int location, int quantity)
    {
        if (inventory.slots[3, location].item.quantity > 0)
        {
            inventory.slots[3, location].item.quantity -= quantity;
            if (inventory.slots[3, location].item.quantity <= 0)
            {
                inventory.slots[3, location].item = null;
            }
        }
        Debug.Log("Da su dung");
    }
}
