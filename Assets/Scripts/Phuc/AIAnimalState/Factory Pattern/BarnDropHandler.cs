using UnityEngine;
using UnityEngine.EventSystems;

public class BarnDropHandler : MonoBehaviour, IDropHandler
{
    public Barn barn;
    public BarnUI barnUI;
    public DragItem dragItem;

    public ItemData hayBaleData;

    public void OnDrop(PointerEventData eventData)
    {
        if (dragItem.draggedItem == null)
            return;

        InventoryItem dragged = dragItem.draggedItem;
        if (dragged.itemData != hayBaleData)
        {
            Notification.Instance.ShowNotification("Chỉ có cỏ khô mới được thêm vào Chuồng Nuôi");
            return;
        }

        if (barnUI.capacity + dragged.quantity > barn.limitCapacity)
        {
            Notification.Instance.ShowNotification("Pen đã đầy!");
            return;
        }

        bool added = barn.AddItem(dragged.itemData, dragged.quantity);
        if (added)
        {
            dragItem.draggedItem = null;
            barnUI.dragIcon.gameObject.SetActive(false);
            barnUI.UpdateAllSlots();
        }
        else
        {
            Notification.Instance.ShowNotification("Pen đã đầy không thể thêm!");
        }
    }
}
