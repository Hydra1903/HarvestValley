using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public Barn barn;
    public InventoryUI inventoryUI1;
    public InventoryUI inventoryUI2;
    public BarnUI barnUI;
    public ItemData item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15;
    public void OnClickSpawnItem()
    {
        if (inventory.AddItem(item1, 1) && inventory.AddItem(item2, 1) && inventory.AddItem(item2, 1) && inventory.AddItem(item3, 99) && inventory.AddItem(item4, 99) && inventory.AddItem(item5, 99) && inventory.AddItem(item6, 99) && inventory.AddItem(item7, 99) && inventory.AddItem(item7, 99) && inventory.AddItem(item8, 99) && inventory.AddItem(item8, 99) && inventory.AddItem(item9, 99) && inventory.AddItem(item10, 99) && inventory.AddItem(item11, 99) && inventory.AddItem(item12, 99) && inventory.AddItem(item13, 99) && inventory.AddItem(item14, 99) && inventory.AddItem(item15, 99))
        {
            inventoryUI1.UpdateAllSlots();
        }
        else
        {
            Notification.Instance.ShowNotification("Túi đồ của bạn đã đầy!");
        }
    }
}

