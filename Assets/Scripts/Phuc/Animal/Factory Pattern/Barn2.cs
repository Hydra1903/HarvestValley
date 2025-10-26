using UnityEngine;

[System.Serializable]
public class BarnSlot2
{
    public GameObject slotObj;
    public int hayAmount = 0; // số cỏ hiện tại trong slot
}

public class Barn2 : MonoBehaviour
{
    public int rows = 1;
    public int columns = 2; // cấp 2 có 2 ô
    public int limitCapacity = 20;

    public BarnSlot2[] slots;

    private void Awake()
    {
        // Khởi tạo mảng slots
        slots = new BarnSlot2[columns];
        for (int i = 0; i < columns; i++)
        {
            slots[i] = new BarnSlot2();
            slots[i].hayAmount = 0;
        }
    }

    public bool AddHay(int slotIndex, int amount)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return false;

        if (slots[slotIndex].hayAmount + amount <= limitCapacity)
        {
            slots[slotIndex].hayAmount += amount;
            return true;
        }

        return false;
    }

    public int GetHayAmount(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return 0;
        return slots[slotIndex].hayAmount;
    }
}