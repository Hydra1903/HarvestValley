using TMPro;
using UnityEngine;

public class Cells : MonoBehaviour
{
    public int currentGrass = 0;
    public int maxGrass = 10;
    public TMP_Text quantityText;

    public void AddGrass(int amount)
    {
        currentGrass = Mathf.Min(currentGrass + amount, maxGrass);
        UpdateUI();
    }

    public void ReduceGrass(int amount)
    {
        currentGrass = Mathf.Max(currentGrass - amount, 0);
        UpdateUI();
    }

    public bool HasGrass() => currentGrass > 0;

    private void UpdateUI()
    {
        if (quantityText != null)
            quantityText.text = currentGrass.ToString();
    }
}
