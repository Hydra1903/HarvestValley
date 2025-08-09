using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform slotsParent;
    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;
    public DragItem dragItem;

    private InventorySlotUI draggingFromSlot;

    public TextMeshProUGUI gold;

    private void Start()
    {
        int totalSlots = inventory.rows * inventory.columns;

        if (slotsParent.childCount != totalSlots)
        {
            return;
        }

        for (int row = 0; row < inventory.rows; row++)
        {
            for (int col = 0; col < inventory.columns; col++)
            {
                int index = row * inventory.columns + col;
                InventorySlotUI slotUI = slotsParent.GetChild(index).GetComponentInChildren<InventorySlotUI>();
                slotUI?.SetSlot(row, col, inventory, this);
            }
        }

        dragIcon.gameObject.SetActive(false);
    }

    public void StartDrag(InventoryItem item, InventorySlotUI fromSlot)
    {
        dragItem.draggedItem = new InventoryItem(item.itemData, item.quantity);
        draggingFromSlot = fromSlot;

        dragIcon.sprite = item.itemData.icon;
        dragQuantityText.text = item.quantity > 0 ? item.quantity.ToString() : "";
        dragIcon.gameObject.SetActive(true);
    }

    public void UpdateDragPosition(Vector2 position)
    {
        dragIcon.transform.position = position;
    }

    public void EndDrag()
    {
        if (dragItem.draggedItem != null && draggingFromSlot != null)
        {
            inventory.slots[draggingFromSlot.row, draggingFromSlot.column].item = dragItem.draggedItem;
        }

        dragItem.draggedItem = null;
        dragIcon.gameObject.SetActive(false);
        UpdateAllSlots();
    }

    public void UpdateAllSlots()
    {
        foreach (var slotUI in slotsParent.GetComponentsInChildren<InventorySlotUI>())
        {
            if (slotUI != null)
            {
                slotUI.UpdateSlotUI();
            }
        }
    }
}
