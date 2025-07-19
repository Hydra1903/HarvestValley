using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarnUI : MonoBehaviour
{
    public Barn barn;
    public Transform slotsParent;

    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;

    public DragItem draggedItem;
    private BarnSlotUI draggingFromSlot;



    private void Start()
    {
        int totalSlots = barn.rows * barn.columns;

        if (slotsParent.childCount != totalSlots)
        {
            Debug.Log("Slot khong dung so luong");
            return;
        }

        for (int row = 0; row < barn.rows; row++)
        {
            for (int col = 0; col < barn.columns; col++)
            {
                int index = row * barn.columns + col;
                Transform slot = slotsParent.GetChild(index);
                BarnSlotUI slotUI = slot.GetComponentInChildren<BarnSlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetSlot(row, col, barn, this);
                }
            }
        }
    }

    public void UpdateAllSlots()
    {
        foreach (var slotUI in slotsParent.GetComponentsInChildren<BarnSlotUI>())
        {
            slotUI.UpdateSlotUI();
        }
    }

    public void StartDrag(InventoryItem item, BarnSlotUI fromSlot)
    {
        draggedItem.draggedItem = new InventoryItem(item.itemData, item.quantity);
        draggingFromSlot = fromSlot;
        dragIcon.sprite = item.itemData.icon;
        dragQuantityText.text = item.quantity > 0 ? item.quantity.ToString() : "";
        dragIcon.gameObject.SetActive(true);

    }

    public void UpdateDragPosition(Vector2 pos)
    {
        dragIcon.transform.position = pos;
    }

    public void EndDrag()
    {
        if (draggedItem != null)
        {
            draggingFromSlot.barn.slots[draggingFromSlot.row, draggingFromSlot.column].item = draggedItem.draggedItem;
        }

        draggedItem = null;
        dragIcon.gameObject.SetActive(false);
        UpdateAllSlots();
    }
}
