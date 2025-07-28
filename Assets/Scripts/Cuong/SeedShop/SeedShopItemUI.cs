using UnityEngine;
using UnityEngine.UI;

public class SeedShopItemUI : MonoBehaviour
{
    public int itemIndex;
    public Text amountText;
    public Text priceText;
    public Button plus1Btn, minus1Btn, plus10Btn, minus10Btn;
    public SeedShop shop;
    private int totalPrice;
    private void Start()
    {
        plus1Btn.onClick.AddListener(() => ChangeAmount(1));
        minus1Btn.onClick.AddListener(() => ChangeAmount(-1));
        plus10Btn.onClick.AddListener(() => ChangeAmount(10));
        minus10Btn.onClick.AddListener(() => ChangeAmount(-10));

        totalPrice = shop.amount[itemIndex] * shop.price[itemIndex];
        UpdateUI();
    }

    public void ChangeAmount(int delta)
    {
        shop.amount[itemIndex] = Mathf.Clamp(shop.amount[itemIndex] + delta, 1, 99);
        totalPrice = shop.amount[itemIndex] * shop.price[itemIndex];
        UpdateUI();
    } 

    private void UpdateUI()
    {
        amountText.text = shop.amount[itemIndex].ToString();
        priceText.text = totalPrice.ToString();
    }

    public void BuyThisItem()
    {
        shop.BuyItem(itemIndex);
        UpdateUI();
    }
}

