using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUI inventoryUI;
    public ItemData item1, item2;

    void Start()
    {
        if (inventory.AddItem(item1, 10) && inventory.AddItem(item2, 15))
        {
            Debug.Log("Them vat pham thanh cong");
            inventoryUI.UpdateAllSlots();
        }
        else
        {
            Debug.Log("Tui do bi day");
        }
      
    }
}

