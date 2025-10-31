using System.Collections.Generic;
using UnityEngine;

public class HayCellManager : MonoBehaviour
{
    [Header("Optional: Use parent to auto-find cells")]
    public Transform cellsParent; // Nếu bạn muốn tự động tìm các cell con
    public DragItem dragItem;
    public Sprite hayBaleIcon;

    [Header("Cells List (can drag manually)")]
    public List<HayCell> hayCells = new List<HayCell>(); // Kéo trực tiếp cell vào đây

    private void Start()
    {
        if ((hayCells == null || hayCells.Count == 0) && cellsParent != null)
        {
            hayCells = new List<HayCell>();
            foreach (var cell in cellsParent.GetComponentsInChildren<HayCell>(true))
            {
                if (cell != null)
                    hayCells.Add(cell);
            }
        }

        foreach (var cell in hayCells)
        {
            if (cell != null)
            {
                cell.manager = this;
                cell.UpdateUI();
            }
        }
    }

    // Load tất cả cells từ FarmSaveData
    public void LoadAllCells(FarmSaveData data)
    {
        foreach (var cell in hayCells)
        {
            if (cell != null)
                cell.LoadHayCell(data);
        }
    }

    // Save tất cả cells vào FarmSaveData
    public void SaveAllCells(FarmSaveData data)
    {
        foreach (var cell in hayCells)
        {
            if (cell != null)
                cell.SaveHayCell(data);
        }
    }

    public bool HasHay()
    {
        foreach (var cell in hayCells)
            if (cell != null && !cell.IsEmpty) return true;
        return false;
    }

    public int TotalHayCount()
    {
        int total = 0;
        foreach (var cell in hayCells)
        {
            if (cell != null)
                total += cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
        }
        return total;
    }
}
