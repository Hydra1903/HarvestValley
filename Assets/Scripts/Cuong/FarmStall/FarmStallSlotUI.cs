using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FarmStallSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public int location;
    public ReceiveItem receiveItem;
    public ReceiveItemUI receiveItemUI;
    public InventoryItem item;

    public ItemData itemData;
    public void SetSlot(int lcn, ReceiveItem rcv, ReceiveItemUI ui)
    {
        location = lcn;
        receiveItem = rcv;
        receiveItemUI = ui;
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        var slot = receiveItem.slots[location];

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
        var slot = receiveItem.slots[location];
        if (!slot.IsEmpty)
        {
            receiveItemUI.StartDrag(slot.item, this);
            slot.item = null;
            UpdateSlotUI();
        }
        else
        {
            Debug.Log("Rong");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        receiveItemUI.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        receiveItemUI.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggingItem = receiveItemUI.dragItem?.draggedItem;
        if (draggingItem == null) return;

        if (draggingItem.itemData != itemData)
        {
            Notification.Instance.ShowNotification("Vật phẩm không hợp lệ!");
            return;
        }
        var targetSlot = receiveItem.slots[location];

        if (targetSlot.IsEmpty)
        {
            targetSlot.item = draggingItem;
            receiveItemUI.dragItem.draggedItem = null;
        }
        else if (targetSlot.item.itemData == draggingItem.itemData && !targetSlot.item.IsFull)
        {
            int canAdd = Mathf.Min(draggingItem.quantity, draggingItem.itemData.maxStack - targetSlot.item.quantity);
            targetSlot.item.quantity += canAdd;
            draggingItem.quantity -= canAdd;

            if (draggingItem.quantity <= 0)
                receiveItemUI.dragItem.draggedItem = null;
        }
        else
        {
            var temp = targetSlot.item;
            targetSlot.item = draggingItem;
            receiveItemUI.dragItem.draggedItem = temp;
        }

        receiveItemUI.UpdateAllSlots();
    }


}
