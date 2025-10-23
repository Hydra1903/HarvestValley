using UnityEngine;
using UnityEngine.EventSystems;

public class BarnDropHandler : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Barn barn;                    // nếu có chứa vật phẩm chung
    public BarnUI barnUI;                // UI tổng
    public DragItem dragItem;            // item đang kéo
    public ItemData hayBaleData;         // item Cỏ khô
    public GettingHayhleCell hayFeedSystem; // hệ thống cell

    public void OnDrop(PointerEventData eventData)
    {
        if (dragItem.draggedItem == null)
            return;

        InventoryItem dragged = dragItem.draggedItem;

        // chỉ chấp nhận Hay Bale
        if (dragged.itemData != hayBaleData)
        {
            Notification.Instance.ShowNotification("Chỉ có cỏ khô mới được thêm vào Chuồng Nuôi");
            return;
        }

        // kiểm tra giới hạn chứa tổng (nếu có)
        if (barn != null && barnUI != null && barnUI.capacity + dragged.quantity > barn.limitCapacity)
        {
            Notification.Instance.ShowNotification("Chuồng đã đầy!");
            return;
        }

        // nếu có hệ thống chia cell
        if (hayFeedSystem != null && hayFeedSystem.feedCells.Count > 0)
        {
            int perCell = dragged.quantity / hayFeedSystem.feedCells.Count;
            int remainder = dragged.quantity % hayFeedSystem.feedCells.Count;

            for (int i = 0; i < hayFeedSystem.feedCells.Count; i++)
            {
                int addAmount = perCell + (i == 0 ? remainder : 0);
                if (addAmount > 0)
                    hayFeedSystem.AddGrassToCell(i, addAmount);
            }

            Notification.Instance.ShowNotification($"Đã thêm {dragged.quantity} cỏ khô vào chuồng!");
        }
        else if (barn != null)
        {
            // fallback nếu không có hệ thống cell
            bool added = barn.AddItem(dragged.itemData, dragged.quantity);
            if (!added)
            {
                Notification.Instance.ShowNotification("Chuồng đã đầy không thể thêm!");
                return;
            }
        }

        // xóa item kéo
        dragItem.draggedItem = null;
        if (barnUI != null)
        {
            barnUI.dragIcon.gameObject.SetActive(false);
            barnUI.UpdateAllSlots(); // cập nhật lại hiển thị
        }

        // cập nhật số cỏ trên từng cell
        if (hayFeedSystem != null)
            hayFeedSystem.UpdateAllCellUI();
    }
}
