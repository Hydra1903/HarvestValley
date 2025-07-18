using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public int row, column;
    public Inventory inventory;
    public InventoryUI inventoryUI;

    public void UpdateSlotUI()
    {
        var slot = inventory.slots[row, column];
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
        var slot = inventory.slots[row, column];
        if (!slot.IsEmpty)
        {
            inventoryUI.StartDrag(slot.item, this);
            slot.item = null;
            UpdateSlotUI();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventoryUI.draggedItem != null)
        {
            var targetSlot = inventory.slots[row, column];
            var dragging = inventoryUI.draggedItem;

            if (targetSlot.IsEmpty)
            {
                targetSlot.item = dragging;
                inventoryUI.draggedItem = null;
            } // THẢ VÀO CHỖ TRỐNG
            else if (targetSlot.item.itemData == dragging.itemData && !targetSlot.item.IsFull)
            {
                int canAdd = Mathf.Min(dragging.quantity, dragging.itemData.maxStack - targetSlot.item.quantity);
                targetSlot.item.quantity += canAdd;
                dragging.quantity -= canAdd;

                if (dragging.quantity <= 0)
                    inventoryUI.draggedItem = null;
            } // THẢ VÀO CHỖ CÓ CÙNG KIỂU ITEM
            else
            {
                inventoryUI.draggedItem = targetSlot.item;
                targetSlot.item = dragging;
            } // THẢ VÀO CHỖ KHÁC KIỂU ITEM ĐỂ ĐỔI CHỖ VỚI NHAU

            inventoryUI.UpdateAllSlots();
        }
    }
    public void SetSlot(int row, int col, Inventory inventory, InventoryUI inventoryUI)
    {
        this.row = row;
        this.column = col;
        this.inventory = inventory;
        this.inventoryUI = inventoryUI;
        UpdateSlotUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowSplitMenu();
        }
    }
    void ShowSplitMenu()
    {
        var slot = inventory.slots[row, column];
        if (slot.item != null)
        {
            SplitMenuUI.Instance.Show(this);
        }
    }
}

