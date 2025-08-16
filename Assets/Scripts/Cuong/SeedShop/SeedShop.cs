using UnityEngine;
using UnityEngine.Playables;

public class SeedShop : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUI inventoryUI;
    public ItemData[] data;
    public int[] amount;
    public int[] price;

    public void BuyItem(int index)
    {
        if (inventory.AddItem(data[index], amount[index]))
        {
            inventoryUI.UpdateAllSlots();
        }
        else
        {
            Notification.Instance.ShowNotification("Túi đồ của bạn đã đầy!");
        }
    }
}
