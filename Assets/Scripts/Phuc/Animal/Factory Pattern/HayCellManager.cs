using System.Collections.Generic;
using UnityEngine;

public class HayCellManager : MonoBehaviour
{
    public Transform cellsParent;
    public DragItem dragItem;

    [HideInInspector] public List<HayCell> hayCells = new List<HayCell>();

    private void Start()
    {
        hayCells.Clear();
        HayCell[] cells = cellsParent.GetComponentsInChildren<HayCell>(true);
        foreach (var cell in cells)
        {
            cell.manager = this;
            hayCells.Add(cell);
            cell.UpdateUI();
        }
    }

    public void LoadAllCells()
    {
        foreach (var cell in hayCells)
        {
            cell.LoadHaybalePen();
            cell.UpdateUI();
        }
    }

    public void UpdateAllCellsUI()
    {
        foreach (var cell in hayCells)
        {
            cell.UpdateUI();
        }
    }

    public bool HasHay()
    {
        foreach (var cell in hayCells)
            if (!cell.isEmpty) return true;
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
