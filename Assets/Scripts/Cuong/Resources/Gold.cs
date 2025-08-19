using System.Globalization;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public static Gold Instance;
    public int gold = 0;


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
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }
}
