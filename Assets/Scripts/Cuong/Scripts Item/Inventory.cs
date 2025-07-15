using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 32;
    public List<InventoryItem> items = new List<InventoryItem>();
    public bool AddItem(ItemData data, int amount)
    {
        foreach (var item in items)
        {
            if (item.itemData == data && item.quantity < data.maxStack)
            {
                int add = Mathf.Min(amount, data.maxStack - item.quantity);
                item.quantity += add;
                amount -= add;
                if (amount <= 0) return true;
            }
        }

        while (amount > 0 && items.Count < maxSlots)
        {
            int add = Mathf.Min(amount, data.maxStack);
            items.Add(new InventoryItem(data, add));
            amount -= add;
        }

        return amount <= 0;
    }

    public void SwapItems(int indexA, int indexB)
    {
        var temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;
    }
}
