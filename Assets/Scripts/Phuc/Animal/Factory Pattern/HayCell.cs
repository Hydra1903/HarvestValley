using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class HayCell : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    [Header("Cell Info")]
    public int maxCapacity = 20;

    [Header("References")]
    public HayCellManager manager;
    public int cellIndex; // 0 hoặc 1
    public int locationPen; // 1 hoặc 2

    public int quanlityCell1;
    public int quanlityCell2;

    public int CurrentQuantity => cellIndex == 0 ? quanlityCell1 : quanlityCell2;
    public bool IsEmpty => CurrentQuantity <= 0;

    private void Start() => UpdateUI();

    public void OnDrop(PointerEventData eventData)
    {
        if (manager?.dragItem?.draggedItem == null) return;
        var dragged = manager.dragItem.draggedItem;
        if (dragged.itemData.itemName != "Hay Bale") return;

        int canAdd = maxCapacity - CurrentQuantity;
        int addAmount = Mathf.Min(canAdd, dragged.quantity);
        if (addAmount <= 0) return;

        if (cellIndex == 0) quanlityCell1 += addAmount;
        else quanlityCell2 += addAmount;

        dragged.quantity -= addAmount;
        if (dragged.quantity <= 0) manager.dragItem.draggedItem = null;

        if (manager != null) itemIcon.sprite = manager.hayBaleIcon;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (itemIcon != null) itemIcon.gameObject.SetActive(!IsEmpty);
        if (quantityText != null) quantityText.text = $"{CurrentQuantity}/{maxCapacity}";
    }

    public void SaveHayCell(FarmSaveData data)
    {
        if (data == null) return;

        data.hayCells.RemoveAll(h => h.penId == locationPen);

        data.hayCells.Add(new HayCellData
        {
            penId = locationPen,
            quantities = new int[2] { quanlityCell1, quanlityCell2 }
        });
    }

    public void LoadHayCell(FarmSaveData data)
    {
        if (data == null || data.hayCells == null) return;

        var saved = data.hayCells.Find(h => h.penId == locationPen);
        if (saved != null)
        {
            quanlityCell1 = saved.quantities[0];
            quanlityCell2 = saved.quantities[1];
        }

        if (CurrentQuantity > 0 && manager != null)
            itemIcon.sprite = manager.hayBaleIcon;

        UpdateUI();
    }
}
