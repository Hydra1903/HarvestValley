using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MerchantSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public int location;
    public ReceiveItem receiveItem;
    public MerchantReceiveItemUI merchantReceiveItemUI;
    public InventoryItem item;

    public ItemData itemData;
    public void SetSlot(int lcn, ReceiveItem rcv, MerchantReceiveItemUI ui)
    {
        location = lcn;
        receiveItem = rcv;
        merchantReceiveItemUI = ui;
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
            merchantReceiveItemUI.StartDrag(slot.item, this);
            slot.item = null;
            UpdateSlotUI();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        merchantReceiveItemUI.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        merchantReceiveItemUI.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggingItem = merchantReceiveItemUI.dragItem?.draggedItem;
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
            merchantReceiveItemUI.dragItem.draggedItem = null;
        }
        else if (targetSlot.item.itemData == draggingItem.itemData && !targetSlot.item.IsFull)
        {
            int canAdd = Mathf.Min(draggingItem.quantity, draggingItem.itemData.maxStack - targetSlot.item.quantity);
            targetSlot.item.quantity += canAdd;
            draggingItem.quantity -= canAdd;

            if (draggingItem.quantity <= 0)
                merchantReceiveItemUI.dragItem.draggedItem = null;
        }
        else
        {
            var temp = targetSlot.item;
            targetSlot.item = draggingItem;
            merchantReceiveItemUI.dragItem.draggedItem = temp;
        }

        merchantReceiveItemUI.UpdateAllSlots();
    }
}
