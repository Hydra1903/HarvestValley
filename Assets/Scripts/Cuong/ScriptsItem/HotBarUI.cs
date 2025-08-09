using TMPro;
using UnityEngine;

public class HotBarUI : MonoBehaviour
{
    public HotBar hotbar;
    public Transform slotsParent;

    private void Start()
    {
        if (slotsParent.childCount != 8)
        {
            return;
        }

        for (int i = 0; i < 8; i++)
        {
            HotBarSlotUI slotUI = slotsParent.GetChild(i).GetComponentInChildren<HotBarSlotUI>();
            slotUI?.SetSlot(i, hotbar, this);
            Debug.Log("Gán" + i);
        }
    }
    public void UpdateAllSlots()
    {
        foreach (var slotUI in slotsParent.GetComponentsInChildren<HotBarSlotUI>())
        {
            slotUI.UpdateSlotUI();
        }
    }
}
