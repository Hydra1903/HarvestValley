using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public ItemData item1, item2;

    void Start()
    {
        inventory.AddItem(item1, 5);
        inventory.AddItem(item2, 10);
        //FindObjectOfType<InventoryUI>().UpdateUI();
    }
}

