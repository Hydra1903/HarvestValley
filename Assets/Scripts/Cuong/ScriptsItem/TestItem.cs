using UnityEngine;

public class TestItem : MonoBehaviour
{
    public ItemData ItemData1;
    public ItemData ItemData2;
    public ItemData ItemData3;
    public Inventory Inventory;
    public void SpawnItem()
    {
        Inventory.AddItem(ItemData1, 6);
        Inventory.AddItem(ItemData2, 6);
        Inventory.AddItem(ItemData3, 6);
    }
}