using System;
using TMPro;
using UnityEngine;

public class HotBarUI : MonoBehaviour
{
    public HotBar hotbar;
    public Transform slotsParent;

    public int valueScroll = 0;
    public int minValue = 0;
    public int maxValue = 7;

    public GameObject[] frameHighlight;
    public int currentHighlightIndex = 0;

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
        if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.idleState ||
            CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.walkState ||
            CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.runState)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll < 0)
            {
                valueScroll++;
                if (valueScroll > maxValue) valueScroll = minValue;
                UpdateCurrentItem(valueScroll);
                UpdateFrameHighlight(valueScroll);
            }
            else if (scroll > 0)
            {
                valueScroll--;
                if (valueScroll < minValue) valueScroll = maxValue;
                UpdateCurrentItem(valueScroll);
                UpdateFrameHighlight(valueScroll);
            }
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
