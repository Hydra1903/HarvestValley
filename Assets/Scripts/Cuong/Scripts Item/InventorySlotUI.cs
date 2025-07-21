using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public int row, column;
    public Inventory inventory;
    public InventoryUI inventoryUI;

    public InventoryItem item;

    public void SetSlot(int r, int c, Inventory inv, InventoryUI ui)
    {
        row = r;
        column = c;
        inventory = inv;
        inventoryUI = ui;
        UpdateSlotUI();
    }

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
        var draggingItem = inventoryUI.dragItem?.draggedItem;
        if (draggingItem == null) return;

        var targetSlot = inventory.slots[row, column];

        if (targetSlot.IsEmpty)
        {
            targetSlot.item = draggingItem;
            inventoryUI.dragItem.draggedItem = null;
        }
        else if (targetSlot.item.itemData == draggingItem.itemData && !targetSlot.item.IsFull)
        {
            int canAdd = Mathf.Min(draggingItem.quantity, draggingItem.itemData.maxStack - targetSlot.item.quantity);
            targetSlot.item.quantity += canAdd;
            draggingItem.quantity -= canAdd;

            if (draggingItem.quantity <= 0)
                inventoryUI.dragItem.draggedItem = null;
        }
        else
        {
            var temp = targetSlot.item;
            targetSlot.item = draggingItem;
            inventoryUI.dragItem.draggedItem = temp;
        }

        inventoryUI.UpdateAllSlots();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var slot = inventory.slots[row, column];
            if (slot.item != null)
            {
                SplitMenuUI.Instance?.Show(this, GetComponent<RectTransform>());
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var slot = inventory.slots[row, column];
        if (!slot.IsEmpty)
        {
            TooltipUI.Instance.Show(
                slot.item.itemData.name,
                slot.item.itemData.season,
                slot.item.itemData.description,
                slot.item.itemData.itemType.ToString(),
                slot.item.itemData.icon,
                slot.item.itemData.itemType
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }

}
