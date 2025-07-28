using UnityEngine;

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
            Debug.Log("tui do da day");
        }
    }
}
