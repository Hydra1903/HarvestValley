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
        if (barnUI.draggedItem != null)
        {
            var targetSlot = barn.slots[row, column];
            var dragging = barnUI.draggedItem.draggedItem;

            if (targetSlot.IsEmpty)
            {
                targetSlot.item = dragging;
                barnUI.draggedItem = null;
            } // THẢ VÀO CHỖ TRỐNG
            else if (targetSlot.item.itemData == dragging.itemData && !targetSlot.item.IsFull)
            {
                int canAdd = Mathf.Min(dragging.quantity, dragging.itemData.maxStack - targetSlot.item.quantity);
                targetSlot.item.quantity += canAdd;
                dragging.quantity -= canAdd;

                if (dragging.quantity <= 0)
                    barnUI.draggedItem = null;
            } // THẢ VÀO CHỖ CÓ CÙNG KIỂU ITEM
            else
            {
                barnUI.draggedItem.draggedItem = targetSlot.item;
                targetSlot.item = dragging;
            } // THẢ VÀO CHỖ KHÁC KIỂU ITEM ĐỂ ĐỔI CHỖ VỚI NHAU

            barnUI.UpdateAllSlots();
        }
        else
        {
            Debug.Log("draggedItemNull");
        }
    }
    public void SetSlot(int row, int col, Barn barn, BarnUI barnUI)
    {
        this.row = row;
        this.column = col;
        this.barn = barn;
        this.barnUI = barnUI;
        UpdateSlotUI();
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowSplitMenu();
        }
    }
    void ShowSplitMenu()
    {
        var slot = barn.slots[row, column];
        if (slot.item != null)
        {
            SplitMenuUI.Instance.Show(this);
        }
    }
    */
}


