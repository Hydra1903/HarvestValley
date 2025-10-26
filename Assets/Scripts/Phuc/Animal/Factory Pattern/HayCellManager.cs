using UnityEngine;
using System.Collections.Generic;

public class HayCellManager : MonoBehaviour
{
    [Header("References")]
    public Transform cellsParent;
    public DragItem dragItem;

    [HideInInspector] public List<HayCell> hayCells = new List<HayCell>();

    private void Start()
    {
        hayCells.Clear();
        HayCell[] cells = cellsParent.GetComponentsInChildren<HayCell>(true); // true = bao gồm inactive
        foreach (var cell in cells)
        {
            cell.manager = this;
            hayCells.Add(cell);
        }

        Debug.Log("HayCells count: " + hayCells.Count);
    }

    public bool HasHay()
    {
        foreach (var cell in hayCells)
            if (cell != null && !cell.isEmpty) return true;
        return false;
    }

    public bool ConsumeHay(int amount = 1)
    {
        foreach (var cell in hayCells)
        {
            if (cell != null && !cell.isEmpty)
            {
                cell.item.quantity -= amount;
                if (cell.item.quantity <= 0)
                {
                    cell.item = null;
                    cell.isEmpty = true;
                }
                cell.UpdateUI();
                return true;
            }
        }
        return false;
    }

    public int TotalHayCount()
    {
        int total = 0;
        foreach (var cell in hayCells)
            if (cell != null && cell.item != null)
                total += cell.item.quantity;
        return total;
    }

    public void UpdateAllCells()
    {
        foreach (var cell in hayCells)
        {
            cell.UpdateUI();
        }
    }
}
