using UnityEngine;
using UnityEngine.EventSystems;

public class BarnDropHandler : MonoBehaviour, IDropHandler
{
    public Barn barn;
    public BarnUI barnUI;
    public DragItem dragItem;
    public ItemData hayBaleData;
    public GettingHayhleCell hayFeedSystem; 

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
            Notification.Instance.ShowNotification("Chuồng đã đầy!");
            return;
        }
        if (hayFeedSystem != null)
        {

            int perCell = dragged.quantity / hayFeedSystem.feedCells.Count;
            int remainder = dragged.quantity % hayFeedSystem.feedCells.Count;

            for (int i = 0; i < hayFeedSystem.feedCells.Count; i++)
            {
                int addAmount = perCell + (i == 0 ? remainder : 0);
                hayFeedSystem.AddGrassToCell(i, addAmount);
            }

            Notification.Instance.ShowNotification($"Đã thêm {dragged.quantity} cỏ khô vào chuồng!");
        }
        else
        {
            bool added = barn.AddItem(dragged.itemData, dragged.quantity);
            if (!added)
            {
                Notification.Instance.ShowNotification("Pen đã đầy không thể thêm!");
                return;
            }
        }

        dragItem.draggedItem = null;
        barnUI.dragIcon.gameObject.SetActive(false);
        barnUI.UpdateAllSlots();
    }
}
