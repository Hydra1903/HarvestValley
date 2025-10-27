using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SeedShopItemUI : MonoBehaviour
{
    public int itemIndex;
    public Text amountText;
    public Text priceText;
    public Button plus1Btn, minus1Btn, plus10Btn, minus10Btn;
    public SeedShop shop;
    private int totalPrice;
    public int levelUnlock;
    public GameObject panelUnlock;
    private void Start()
    {
        plus1Btn.onClick.AddListener(() => ChangeAmount(1));
        minus1Btn.onClick.AddListener(() => ChangeAmount(-1));
        plus10Btn.onClick.AddListener(() => ChangeAmount(10));
        minus10Btn.onClick.AddListener(() => ChangeAmount(-10));
        CalculatePrice();
        UpdateUI();
    }
    public void CalculatePrice()
    {
        if (CharacterStateMachine.Instance.currentCharacter == ECharacter.May)
        {
            totalPrice = (int)Math.Round((float)shop.amount[itemIndex] * shop.price[itemIndex] * 0.9f, MidpointRounding.AwayFromZero);
        }
        else
        {
            totalPrice = shop.amount[itemIndex] * shop.price[itemIndex];
        }

    }
    public void ChangeAmount(int delta)
    {
        shop.amount[itemIndex] = Mathf.Clamp(shop.amount[itemIndex] + delta, 1, 99);
        CalculatePrice();
        UpdateUI();
    } 

    public void UpdateUI()
    {
        amountText.text = shop.amount[itemIndex].ToString();
        priceText.text = totalPrice.ToString("N0", new CultureInfo("de-DE"));
    }

    public void BuyThisItem()
    {
        if (Gold.Instance.gold >= totalPrice)
        {
            Gold.Instance.gold -= totalPrice;
            shop.BuyItem(itemIndex);
            UpdateUI();
        }
        else
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }

    }

    public void UpdateUnlock()
    {
        if (LevelManager.Instance.currentLevel >= levelUnlock)
        {
            panelUnlock.SetActive(false);
        }
    }
}

