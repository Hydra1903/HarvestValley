using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarnUI : MonoBehaviour
{
    public Barn barn;
    public Transform slotsParent;
    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;
    public DragItem dragItem;

    private BarnSlotUI draggingFromSlot;
   // public InventoryItem item;
    public int capacity;
    public TextMeshProUGUI capacityText;

    private void Start()
    {
        int totalSlots = barn.rows * barn.columns;

        if (slotsParent.childCount != totalSlots)
        {
            return;
        }

        for (int row = 0; row < barn.rows; row++)
        {
            for (int col = 0; col < barn.columns; col++)
            {
                int index = row * barn.columns + col;
                BarnSlotUI slotUI = slotsParent.GetChild(index).GetComponentInChildren<BarnSlotUI>();
                slotUI?.SetSlot(row, col, barn, this);
            }
        }
        CountAllItems();
        dragIcon.gameObject.SetActive(false);
    }

    public void StartDrag(InventoryItem item, BarnSlotUI fromSlot)
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
            barn.slots[draggingFromSlot.row, draggingFromSlot.column].item = dragItem.draggedItem;
        }

        dragItem.draggedItem = null;
        dragIcon.gameObject.SetActive(false);
        UpdateAllSlots();
    }

    public void UpdateAllSlots()
    {      
        foreach (var slotUI in slotsParent.GetComponentsInChildren<BarnSlotUI>())
        {
            slotUI.UpdateSlotUI();
        }
        CountAllItems();
    }
    public void CountAllItems()
    {
        capacity = 0;
        for (int r = 0; r < barn.rows; r++)
        {
            for (int c = 0; c < barn.columns; c++)
            {
                if (barn.slots[r, c].item != null)
                {
                    capacity += barn.slots[r, c].item.quantity;
                }
            }
        }
        capacityText.text = capacity.ToString() + "/" + barn.limitCapacity.ToString();
    }
}
