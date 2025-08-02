using System.Globalization;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Gold : MonoBehaviour
{
    public static Gold Instance;

    public int gold = 0;

    public InventoryUI inventoryUI;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI(); 
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    private void UpdateGoldUI()
    {
        inventoryUI.gold.text = gold.ToString("N0", new CultureInfo("de-DE"));
    }
}
