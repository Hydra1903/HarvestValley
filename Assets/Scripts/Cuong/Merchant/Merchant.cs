using UnityEngine;

public class Merchant : MonoBehaviour
{
    public int[] quantity;
    public int[] salesLimit;
    public int[] quantityItemsSold;
    public int[] bonusSellPrice;
    public int[] sellPriceAnimalProduct;
    public int totalAmount;

    public MerchantUI merchantUI;
    public FarmStall farmStall;
    void Awake()
    {
        quantity = new int[32];
        quantityItemsSold = new int[32];
    }
    public void TotalAmount()
    {
        totalAmount = 0;
        for (int i = 0; i < 28; i++)
        {
            switch (Season.Instance.currentSeason)
            {
                case SeasonState.Spring:
                    totalAmount += (farmStall.sellPriceSpring[i] + bonusSellPrice[i]) * quantity[i];
                    break;
                case SeasonState.Summer:
                    totalAmount += (farmStall.sellPriceSummer[i] + bonusSellPrice[i]) * quantity[i];
                    break;
                case SeasonState.Fall:
                    totalAmount += (farmStall.sellPriceFall[i] + bonusSellPrice[i]) * quantity[i];
                    break;
                case SeasonState.Winter:
                    totalAmount += (farmStall.sellPriceWinter[i] + bonusSellPrice[i]) * quantity[i];
                    break;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            totalAmount += (sellPriceAnimalProduct[i]) * quantity[28 + i];
        }
        merchantUI.UpdateUI();
    }
    public void AddQuantityItemsSold()
    {
        for (int i = 0; i < quantity.Length; i++)
        {
            quantityItemsSold[i] += quantity[i];
        }
    }
}
