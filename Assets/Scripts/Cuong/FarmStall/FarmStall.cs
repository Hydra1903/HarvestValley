using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class FarmStall : MonoBehaviour
{
    public int[] sellPrice;
    public int[] quantity;

    public int totalAmount;

    public FarmStallUI farmStallUI;

    void Awake()
    {
        quantity = new int[sellPrice.Length];
    }
    public void TotalAmount()
    {
        totalAmount = 0;
        for (int i = 0; i < sellPrice.Length; i++)
        {
            totalAmount += sellPrice[i] * quantity[i];
        }
        farmStallUI.UpdateUI();
    }
}
