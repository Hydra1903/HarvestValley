using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class FarmStall : MonoBehaviour
{
    public int[] sellPriceSpring;
    public int[] quantity;

    public int totalAmount;

    public FarmStallUI farmStallUI;

    void Awake()
    {
        quantity = new int[sellPriceSpring.Length];
    }
    public void TotalAmount()
    {
        totalAmount = 0;
        for (int i = 0; i < sellPriceSpring.Length; i++)
        {
            totalAmount += sellPriceSpring[i] * quantity[i];
        }
        farmStallUI.UpdateUI();
    }
}
