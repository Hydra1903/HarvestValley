<<<<<<< HEAD
﻿using System.IO;
using TMPro;
using UnityEditor.Overlays;
=======
﻿using System;
using System.Collections.Generic;
using TMPro;
>>>>>>> parent of a32a06d (update)
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class HayCellSaveData
{
<<<<<<< HEAD
    public string itemName;
    public int[] quantityPen1 = new int[2];
    public int[] quantityPen2 = new int[2];
=======
    public string itemName; // "Hay Bale"
    public int quantity;
    public int cellIndex;
}

[Serializable]
public class HayCellManagerSaveData
{
    public int penId;
    public List<HayCellSaveData> cells = new List<HayCellSaveData>();
>>>>>>> parent of a32a06d (update)
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
<<<<<<< HEAD
        if (dragged.quantity <= 0) manager.dragItem.draggedItem = null;
        itemIcon.sprite = manager.hayBaleIcon;
        SaveLoadSystem.haybaler.itemName = "Hay Bale";
=======
        if (dragged.quantity <= 0)
            manager.dragItem.draggedItem = null;
>>>>>>> parent of a32a06d (update)

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
<<<<<<< HEAD

        string json = JsonUtility.ToJson(SaveLoadSystem.haybaler, true);
        File.WriteAllText(SaveLoadSystem.savePath, json);
        Debug.Log($"✅ Farm saved to: {SaveLoadSystem.savePath}");
    }

    public void LoadHaybalePen()
    {
        if (!File.Exists(SaveLoadSystem.savePath)) return;

        string json = File.ReadAllText(SaveLoadSystem.savePath);
        SaveLoadSystem.haybaler = JsonUtility.FromJson<HayCellSaveData>(json);

        if (locationPen == 1)
        {
            quanlityCell1 = SaveLoadSystem.haybaler.quantityPen1[0];
            quanlityCell2 = SaveLoadSystem.haybaler.quantityPen1[1];
        }
        else if (locationPen == 2)
        {
            quanlityCell1 = SaveLoadSystem.haybaler.quantityPen2[0];
            quanlityCell2 = SaveLoadSystem.haybaler.quantityPen2[1];
        }
        if (SaveLoadSystem.haybaler.itemName == "Hay Bale" && manager != null)
            itemIcon.sprite = manager.hayBaleIcon;
=======
>>>>>>> parent of a32a06d (update)
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
