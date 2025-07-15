using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI countText;

    private int slotIndex;
    private InventoryUI inventoryUI;

    public void Set(InventoryItem item, int index, InventoryUI ui)
    {
        icon.sprite = item.itemData.icon;
        icon.enabled = true;
        countText.text = item.quantity.ToString();

        slotIndex = index;
        inventoryUI = ui;
    }

    public void Clear()
    {
        icon.enabled = false;
        countText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.SetIcon(icon.sprite);
        inventoryUI.dragSourceIndex = slotIndex;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.Hide();
    }

    public void OnDrop(PointerEventData eventData)
    {
        int from = inventoryUI.dragSourceIndex;
        int to = slotIndex;
        inventoryUI.inventory.SwapItems(from, to);
        inventoryUI.UpdateUI();
    }
}
