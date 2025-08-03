using UnityEngine;

public class Merchant : MonoBehaviour
{
    public int[] quantity;
    public int[] salesLimit;
    public int totalAmount;

    public MerchantUI merchantUI;
    public FarmStall farmStall;
    void Awake()
    {
        quantity = new int[farmStall.sellPriceSpring.Length];
    }
    public void TotalAmount()
    {
        totalAmount = 0;
        for (int i = 0; i < farmStall.sellPriceSpring.Length; i++)
        {
            totalAmount += farmStall.sellPriceSpring[i] * quantity[i];
        }
        merchantUI.UpdateUI();
    }
}
