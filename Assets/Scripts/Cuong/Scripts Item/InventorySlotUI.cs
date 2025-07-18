using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
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
            Debug.Log("Cap nhat hinh anh");
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
            }
            else if (targetSlot.item.itemData == dragging.itemData && !targetSlot.item.IsFull)
            {
                int canAdd = Mathf.Min(dragging.quantity, dragging.itemData.maxStack - targetSlot.item.quantity);
                targetSlot.item.quantity += canAdd;
                dragging.quantity -= canAdd;

                if (dragging.quantity <= 0)
                    inventoryUI.draggedItem = null;
            }
            else
            {
                inventoryUI.draggedItem = targetSlot.item;
                targetSlot.item = dragging;
            }

            UpdateSlotUI();
            inventoryUI.UpdateAllSlots();
        }
    }
    public void SetSlot(int row, int col, Inventory inventory, InventoryUI inventoryUI)
    {
        this.row = row;
        this.column = col;
        this.inventory = inventory;
        this.inventoryUI = inventoryUI;


        UpdateSlotUI(); // Hàm này cập nhật lại icon/amount, nếu có
    }
}

