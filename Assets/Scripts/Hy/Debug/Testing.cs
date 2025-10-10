using UnityEngine;

public class Testing : MonoBehaviour
{
    public Inventory inventory;
    public Barn barn;
    public InventoryUI inventoryUI1;
    public BarnUI barnUI;

    // Use an array to manage all items in one place
    public ItemData[] itemsToSpawn;

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
    }
}