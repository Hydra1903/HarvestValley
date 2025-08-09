using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BarnSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public int row, column;
    public Barn barn;
    public BarnUI barnUI;

    public void SetSlot(int r, int c, Barn bn, BarnUI ui)
    {
        row = r;
        column = c;
        barn = bn;
        barnUI = ui;
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        var slot = barn.slots[row, column];

        if (slot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slot.item.itemData.icon;
            quantityText.text = slot.item.quantity > 0 ? slot.item.quantity.ToString() : "";
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = barn.slots[row, column];
        if (!slot.IsEmpty)
        {
            barnUI.StartDrag(slot.item, this);
            slot.item = null;
            UpdateSlotUI();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        barnUI.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        barnUI.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        int Rq = barn.limitCapacity - barnUI.capacity;
        var draggingItem = barnUI.dragItem?.draggedItem;
        if (draggingItem == null) return;

        var targetSlot = barn.slots[row, column];

        if (targetSlot.IsEmpty)
        {

            if (Rq > 99)
            {
                targetSlot.item = draggingItem;
                barnUI.dragItem.draggedItem = null;
            }
            else
            {
                targetSlot.item = draggingItem;
                targetSlot.item.quantity = Rq;
                barnUI.dragItem.draggedItem.quantity -= Rq;
            }
        }
        else if (targetSlot.item.itemData == draggingItem.itemData && !targetSlot.item.IsFull)
        {
            int canAdd = Mathf.Min(draggingItem.quantity, draggingItem.itemData.maxStack - targetSlot.item.quantity);
            targetSlot.item.quantity += canAdd;
            draggingItem.quantity -= canAdd;

            if (draggingItem.quantity <= 0)
                barnUI.dragItem.draggedItem = null;
        }
        else
        {
            var temp = targetSlot.item;
            targetSlot.item = draggingItem;
            barnUI.dragItem.draggedItem = temp;
        }

        barnUI.UpdateAllSlots();
        Debug.Log(Rq);
    }
    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var slot = inventory.slots[row, column];
            if (slot.item != null)
            {
                SplitMenuUI.Instance?.Show(this);
            }
        }
    }
    */
}


