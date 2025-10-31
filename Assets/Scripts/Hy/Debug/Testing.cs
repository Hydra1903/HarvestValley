using UnityEngine;

public class Testing : MonoBehaviour
{
    public Inventory inventory;
    public Barn barn;
    public InventoryUI inventoryUI1;
    public BarnUI barnUI;
    public ItemData[] itemsToSpawn;

    // Use an array to manage all items in one place
    public ItemData Hoe;
    public ItemData Water;
    public ItemData liem;
    public void OnClickSpawnItem()
    {
        bool allItemsAdded = true;
        int defaultQuantity = 20;

        foreach (ItemData item in itemsToSpawn)
        {
            if (item != null)
            {
                if (!inventory.AddItem(item, defaultQuantity))
                {
                    allItemsAdded = false;
                    break; 
                }
            }
        }

        if (allItemsAdded)
        {
            inventoryUI1.UpdateAllSlots();
            Notification.Instance.ShowNotification("Đã thêm tất cả vật phẩm thành công!");
        }
        else
        {
            Notification.Instance.ShowNotification("Túi đồ của bạn đã đầy!");
        }
        Tool();
    }
    public void Tool()
    {
        inventory.AddItem(Hoe, 0);
        inventory.AddItem(Water, 0);
        inventory.AddItem(liem, 0);
    }
}