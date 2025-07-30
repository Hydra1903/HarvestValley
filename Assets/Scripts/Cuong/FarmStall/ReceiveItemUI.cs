using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReceiveItemUI : MonoBehaviour
{
    public ReceiveItem receiveItem;
    public Transform slotsParent;
    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;
    public DragItem dragItem;

    private FarmSatllSlotUI draggingFromSlot;

    public int capacity;
    public Text capacityText;

    private void Start()
    {
        if (slotsParent.childCount != 2)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            FarmSatllSlotUI slotUI = slotsParent.GetChild(i).GetComponentInChildren<FarmSatllSlotUI>();
            slotUI?.SetSlot(i, receiveItem, this);
        }
    }

    public void StartDrag(InventoryItem item, FarmSatllSlotUI fromSlot)
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
            receiveItem.slots[draggingFromSlot.location].item = dragItem.draggedItem;
        }

        dragItem.draggedItem = null;
        dragIcon.gameObject.SetActive(false);
        UpdateAllSlots();
    }

    public void UpdateAllSlots()
    {
        foreach (var slotUI in slotsParent.GetComponentsInChildren<FarmSatllSlotUI>())
        {
            slotUI.UpdateSlotUI();
        }
        CountAllItems();
    }
    public void CountAllItems()
    {
        capacity = 0;
        for (int i = 0; i < 2; i++)
        {
            if (receiveItem.slots[i].item != null)
            {
                capacity += receiveItem.slots[i].item.quantity;
            }
        }
        capacityText.text = capacity.ToString() + "/198";
    }
}
