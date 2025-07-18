using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    //public GameObject slotPrefab;
    public Transform slotParent;
    public Transform slotsParent; // Để gán "SlotsParent" chứa 32 ô

    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;

    public InventoryItem draggedItem = null;
    private InventorySlotUI draggingFromSlot;

  

    private void Start()
    {
        int totalSlots = inventory.rows * inventory.columns;

        if (slotsParent.childCount != totalSlots)
        {
            Debug.Log("Slot khong dung so luong");
            return;
        }

        for (int row = 0; row < inventory.rows; row++)
        {
            for (int col = 0; col < inventory.columns; col++)
            {
                int index = row * inventory.columns + col;
                Transform slot = slotsParent.GetChild(index);
                InventorySlotUI slotUI = slot.GetComponentInChildren<InventorySlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetSlot(row, col, inventory, this);
                }
            }
        }
    }
    
    public void UpdateAllSlots()
    {
        foreach (var slotUI in slotParent.GetComponentsInChildren<InventorySlotUI>())
        {
            slotUI.UpdateSlotUI();
        }
    }

    public void StartDrag(InventoryItem item, InventorySlotUI fromSlot)
    {
        draggedItem = new InventoryItem(item.itemData, item.quantity);
        draggingFromSlot = fromSlot;
        dragIcon.sprite = item.itemData.icon;
        dragQuantityText.text = item.quantity > 0 ? item.quantity.ToString() : "";
        dragIcon.gameObject.SetActive(true);
    }

    public void UpdateDragPosition(Vector2 pos)
    {
        dragIcon.transform.position = pos;
        dragQuantityText.transform.position = pos;
    }

    public void EndDrag()
    {
        if (draggedItem != null)
        {
            draggingFromSlot.inventory.slots[draggingFromSlot.row, draggingFromSlot.column].item = draggedItem;
        }

        draggedItem = null;
        dragIcon.gameObject.SetActive(false);
        UpdateAllSlots();
    }
}
