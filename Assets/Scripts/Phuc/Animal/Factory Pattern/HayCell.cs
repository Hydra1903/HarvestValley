using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class HayCellSaveData
{
    public string itemName; // "Hay Bale"
    public int quantity;
    public int cellIndex;
}

[Serializable]
public class HayCellManagerSaveData
{
    public int penId;
    public List<HayCellSaveData> cells = new List<HayCellSaveData>();
}

public class HayCell : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    [Header("Cell Info")]
    public InventoryItem item;
    public int maxCapacity = 20;
    public bool isEmpty = true;

    [Header("References")]
    public HayCellManager manager;
    public int cellIndex;

    private void Start()
    {
        UpdateUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (manager == null || manager.dragItem == null || manager.dragItem.draggedItem == null)
            return;

        InventoryItem dragged = manager.dragItem.draggedItem;

        if (dragged.itemData.itemName != "Hay Bale")
            return;

        int canAdd = maxCapacity - (item != null ? item.quantity : 0);
        int addAmount = Mathf.Min(canAdd, dragged.quantity);

        if (addAmount <= 0) return;

        if (isEmpty)
        {
            item = new InventoryItem(dragged.itemData, addAmount);
            isEmpty = false;
        }
        else
        {
            item.quantity += addAmount;
        }

        dragged.quantity -= addAmount;
        if (dragged.quantity <= 0)
            manager.dragItem.draggedItem = null;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (item == null || isEmpty)
        {
            itemIcon.color = new Color(1, 1, 1, 0);
            quantityText.text = "";
            return;
        }

        itemIcon.sprite = item.itemData.icon;
        itemIcon.color = Color.white;
        quantityText.text = $"{item.quantity}/{maxCapacity}";
    }

    // Gán dữ liệu khi load
    public void LoadCell(HayCellSaveData saveData, ItemData hayBaleData)
    {
        if (saveData.quantity <= 0)
        {
            item = null;
            isEmpty = true;
        }
        else
        {
            int qty = Mathf.Min(saveData.quantity, maxCapacity);
            // Tạo InventoryItem mới từ ItemData đã có sẵn
            item = new InventoryItem(hayBaleData, qty);
            isEmpty = false;
        }
        UpdateUI();
    }

    public HayCellSaveData GetSaveData()
    {
        return new HayCellSaveData
        {
            itemName = item != null ? item.itemData.itemName : "Hay Bale",
            quantity = item != null ? item.quantity : 0,
            cellIndex = cellIndex
        };
    }
}
