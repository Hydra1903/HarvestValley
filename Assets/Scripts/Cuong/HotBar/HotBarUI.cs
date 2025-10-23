using System;
using TMPro;
using UnityEngine;

public class HotBarUI : MonoBehaviour
{
    public static HotBarUI Instance;
    public void Awake()
    {
        if (Instance == null) Instance = this;
    }
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
        UpdateAllSlots();
        UpdateCurrentItem(valueScroll);
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
        if (currentItem != null)
        {
            if (currentItem.itemData == WaterCan.Instance.waterCan)
            {
                if (WaterCan.Instance.currentState == EWaterCanState.Off)
                {
                    WaterCan.Instance.currentState = EWaterCanState.On;
                    WaterCan.Instance.CheckCurrentState();
                }
            }
            else
            {
                if (WaterCan.Instance.currentState == EWaterCanState.On)
                {
                    WaterCan.Instance.currentState = EWaterCanState.Off;
                    WaterCan.Instance.CheckCurrentState();
                }
            }
        }
        else
        {
            if (WaterCan.Instance.currentState == EWaterCanState.On)
            {
                WaterCan.Instance.currentState = EWaterCanState.Off;
                WaterCan.Instance.CheckCurrentState();
            }
        }

        if (currentItem != null)
        {
            if (currentItem.itemData == ChangeMode.Instance.hoe)
            {
                if (ChangeMode.Instance.currentState == EChangeModeState.Off)
                {
                    ChangeMode.Instance.currentState = EChangeModeState.On;
                    ChangeMode.Instance.CheckCurrentState();
                }
            }
            else
            {
                if (ChangeMode.Instance.currentState == EChangeModeState.On)
                {
                    ChangeMode.Instance.currentState = EChangeModeState.Off;
                    ChangeMode.Instance.CheckCurrentState();
                }
            }
        }
        else
        {
            if (ChangeMode.Instance.currentState == EChangeModeState.On)
            {
                ChangeMode.Instance.currentState = EChangeModeState.Off;
                ChangeMode.Instance.CheckCurrentState();
            }
        }
        if (currentItem == null)
        {
            if (ChangeInteract.Instance.currentState == EChangeInteractState.Off)
            {
                ChangeInteract.Instance.currentState = EChangeInteractState.On;
                ChangeInteract.Instance.CheckCurrentState();
            }
        }
        else
        {
            if (ChangeInteract.Instance.currentState == EChangeInteractState.On)
            {
                ChangeInteract.Instance.currentState = EChangeInteractState.Off;
                ChangeInteract.Instance.CheckCurrentState();
            }
        }
    }
    public void UseItem()
    {
        hotbar.UseAndRemoveItem(valueScroll, 1);
    }
}
