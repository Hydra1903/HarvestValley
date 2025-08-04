using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public Barn barn;
    public InventoryUI inventoryUI1;
    public InventoryUI inventoryUI2;
    public BarnUI barnUI;
    public ItemData item1, item2, item3, item4, item5;
    public void OnClickSpawnItem()
    {
        if (inventory.AddItem(item1, 10) && inventory.AddItem(item2, 99) && inventory.AddItem(item2, 80) && inventory.AddItem(item3, 0) && inventory.AddItem(item4, 34) && inventory.AddItem(item5, 5))
        {
            inventoryUI1.UpdateAllSlots();
            inventoryUI2.UpdateAllSlots();
        }
        else
        {
            Debug.Log("Tui do bi day");
        }
        /*
        if (barn.AddItem(item1, 10) && barn.AddItem(item2, 23) && barn.AddItem(item2, 80))
        {
            barnUI.UpdateAllSlots();
        }
        else
        {
            Debug.Log("Tui do bi day");
        }
        */
    }
}

