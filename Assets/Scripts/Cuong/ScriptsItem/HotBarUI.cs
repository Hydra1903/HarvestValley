using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HotBarUI : MonoBehaviour
{
    public HotBar hotbar;
    public Transform slotsParent;

    public int valueScroll = 0;
    public int minValue = 0;
    public int maxValue = 7;

    public GameObject[] frameHighlight;
    private int currentHighlightIndex = 0;

    public InventoryItem currentItem;

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

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0)
        {
            valueScroll++;
            if (valueScroll > maxValue) valueScroll = minValue;
            UpdateCurrentItem(valueScroll);
            UpdateFrameHighlight(valueScroll);
        }
        else if (scroll < 0)
        {
            valueScroll--;
            if (valueScroll < minValue) valueScroll = maxValue;
            UpdateCurrentItem(valueScroll);
            UpdateFrameHighlight(valueScroll);
        }
    }

    public void UpdateFrameHighlight(int index)
    {
        frameHighlight[currentHighlightIndex].SetActive(false);

        frameHighlight[index].SetActive(true);

        currentHighlightIndex = index;
    }
    public void UpdateCurrentItem(int index)
    {
        currentItem = hotbar.slots[index].item;
    }

    public void UseItem()
    {
        hotbar.UseAndRemoveItem(valueScroll, 1);
    }
}
