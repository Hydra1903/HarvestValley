using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantReceiveItemUI : MonoBehaviour
{
    public ReceiveItem receiveItem;
    public Transform slotsParent;
    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;
    public DragItem dragItem;

    private MerchantSlotUI draggingFromSlot;

    public int capacity;
    public Text capacityText;

    public Merchant merchant;

    public bool isLimitReached;
    private void Start()
    {
        if (slotsParent.childCount != 2)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            MerchantSlotUI slotUI = slotsParent.GetChild(i).GetComponentInChildren<MerchantSlotUI>();
            slotUI?.SetSlot(i, receiveItem, this);
        }


        capacityText.text = capacity.ToString() + "/" + merchant.salesLimit[receiveItem.locationDataItem].ToString();
    }

    public void StartDrag(InventoryItem item, MerchantSlotUI fromSlot)
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
        foreach (var slotUI in slotsParent.GetComponentsInChildren<MerchantSlotUI>())
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
        capacityText.text = (merchant.quantityItemsSold[receiveItem.locationDataItem] + capacity).ToString() + "/" + merchant.salesLimit[receiveItem.locationDataItem].ToString();
        merchant.quantity[receiveItem.locationDataItem] = capacity;
        merchant.TotalAmount();
    }
}
