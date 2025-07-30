using UnityEngine;

public class ReceiveItem : MonoBehaviour
{
    public InventorySlot[] slots;
    void Awake()
    {
        slots = new InventorySlot[2];
        slots[0] = new InventorySlot();
        slots[1] = new InventorySlot();
    }
}
