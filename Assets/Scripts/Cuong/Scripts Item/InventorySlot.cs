using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler

{
    public Image icon;
    public TextMeshProUGUI countText;

    private int slotIndex;
    private InventoryUI inventoryUI;

    public void Set(InventoryItem item, int index, InventoryUI ui)
    {
        icon.sprite = item.itemData.icon;
        icon.enabled = true;
        if (item.itemData.itemType != ItemType.Tool)
        {
            countText.text = item.quantity.ToString();          
        }
        else
        {
            countText.text = "";
        }
        slotIndex = index;
        inventoryUI = ui;
    }

    public void Clear()
    {
        icon.enabled = false;
        countText.text = "";
    }

    public void OnDrag(PointerEventData eventData)
    {
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
        Debug.Log(inventoryUI.dragSourceIndex +"    "+ slotIndex);
        inventoryUI.inventory.SwapItems(from, to);
        inventoryUI.UpdateUI();
    }
}
