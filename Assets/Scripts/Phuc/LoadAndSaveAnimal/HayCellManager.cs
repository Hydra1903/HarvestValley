using System.Collections.Generic;
using UnityEngine;

public class HayCellManager : MonoBehaviour
{
    public Transform cellsParent;
    public DragItem dragItem;
    public Sprite hayBaleIcon;

    [HideInInspector] public List<HayCell> hayCells = new List<HayCell>();

    private void Start()
    {
        hayCells.Clear();
        foreach (var cell in cellsParent.GetComponentsInChildren<HayCell>(true))
        {
            cell.manager = this;
            hayCells.Add(cell);
            cell.UpdateUI();
        }
    }

    // Load tất cả cells từ FarmSaveData
    public void LoadAllCells(FarmSaveData data)
    {
        foreach (var cell in hayCells)
            cell.LoadHayCell(data);
    }

    // Save tất cả cells vào FarmSaveData
    public void SaveAllCells(FarmSaveData data)
    {
        foreach (var cell in hayCells)
            cell.SaveHayCell(data);
    }

    public bool HasHay()
    {
        foreach (var cell in hayCells)
            if (!cell.IsEmpty) return true;
        return false;
    }

    public int TotalHayCount()
    {
        int total = 0;
        foreach (var cell in hayCells)
            total += cell.cellIndex == 0 ? cell.quanlityCell1 : cell.quanlityCell2;
        return total;
    }
}
