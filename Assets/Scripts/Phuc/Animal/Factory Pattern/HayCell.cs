using System.IO;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class HayCellSaveData
{
    public string itemName;
    public int[] quantityPen1 = new int[2];
    public int[] quantityPen2 = new int[2];
}

public class HayCell : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    [Header("Cell Info")]
    public int maxCapacity = 20;
    public bool isEmpty = true;

    [Header("References")]
    public HayCellManager manager;
    public int cellIndex; // 0 hoặc 1
    public int quanlityCell1;
    public int quanlityCell2;
    public int locationPen; // 1 hoặc 2

    private void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (manager == null || manager.dragItem == null || manager.dragItem.draggedItem == null)
            return;

        var dragged = manager.dragItem.draggedItem;
        if (dragged.itemData.itemName != "Hay Bale") return;

        int canAdd = maxCapacity - (cellIndex == 0 ? quanlityCell1 : quanlityCell2);
        int addAmount = Mathf.Min(canAdd, dragged.quantity);
        if (addAmount <= 0) return;

        if (cellIndex == 0) quanlityCell1 += addAmount;
        else quanlityCell2 += addAmount;

        dragged.quantity -= addAmount;
        if (dragged.quantity <= 0) manager.dragItem.draggedItem = null;
        itemIcon.sprite = manager.hayBaleIcon;
        SaveLoadSystem.haybaler.itemName = "Hay Bale";

        UpdateUI();
    }

    public void UpdateUI()
    {
        itemIcon.gameObject.SetActive(true);
        quantityText.gameObject.SetActive(true);

        int qty = cellIndex == 0 ? quanlityCell1 : quanlityCell2;
        quantityText.text = $"{qty}/{maxCapacity}";
    }

    public void SaveHaybalePen()
    {
        if (locationPen == 1)
        {
            SaveLoadSystem.haybaler.quantityPen1[0] = quanlityCell1;
            SaveLoadSystem.haybaler.quantityPen1[1] = quanlityCell2;
        }
        else if (locationPen == 2)
        {
            SaveLoadSystem.haybaler.quantityPen2[0] = quanlityCell1;
            SaveLoadSystem.haybaler.quantityPen2[1] = quanlityCell2;
        }

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
        UpdateUI();
    }
}
