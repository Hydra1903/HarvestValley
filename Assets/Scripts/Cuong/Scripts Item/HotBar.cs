using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class HotBar : MonoBehaviour
{
    public InventorySlot[] slots;
    public Inventory inventory;
    void Awake()
    {
        slots = new InventorySlot[8];

        for (int i = 0; i < 8; i++)
        {
            slots[i] = new InventorySlot();
            Debug.Log("Khoi Tao");
        }
        UpdateData();
    }
    public void UpdateData()
    {
        for (int i = 0; i < 8; i++)
        {
            slots[i] = inventory.slots[3,i];
            Debug.Log("Cap nhat");
        }

    }
}
