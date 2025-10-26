using System.Collections.Generic;
using UnityEngine;

public class GettingHayhleCell : MonoBehaviour
{
    [Header("Feed Cells (the grass cells for this pen)")]
    public List<Cells> feedCells = new List<Cells>();
    public void UpdateAllCellUI()
    {
        for (int i = 0; i < feedCells.Count; i++)
        {
            var cell = feedCells[i];
            cell.UpdateUI(); // m?i cell có TMP riêng ð? hi?n th? s? c?
        }
    }
    public bool ConsumeGrass(int amount = 1)
    {
        foreach (var cell in feedCells)
        {
            if (cell.HasGrass())
            {
                cell.ReduceGrass(amount);
                if (cell.HasGrass())
                    return true;
                continue;
            }
        }

        return false;
    }

    public bool HasAnyGrass()
    {
        foreach (var cell in feedCells)
        {
            if (cell.HasGrass())
                return true;
        }
        return false;
    }

    public void AddGrassToCell(int cellIndex, int amount)
    {
        if (cellIndex < 0 || cellIndex >= feedCells.Count)
            return;

        feedCells[cellIndex].AddGrass(amount);
    }
    public void AddGrassAll(int amountPerCell)
    {
        foreach (var cell in feedCells)
        {
            cell.AddGrass(amountPerCell);
        }
    }
    public int GetTotalGrass()
    {
        int total = 0;
        foreach (var cell in feedCells)
            total += cell.currentGrass;
        return total;
    }
}
