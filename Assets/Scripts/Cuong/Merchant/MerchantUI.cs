using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public enum MerchantState
{
    NotForSale,
    ReadyForSale
}
public class MerchantUI : MonoBehaviour
{
    public Merchant merchant;
    public TextMeshProUGUI totalAmountText;
    public Button buttonSell;
    public MerchantState currentState = MerchantState.NotForSale;

    public ReceiveItem[] receiveItems;
    public MerchantReceiveItemUI[] merchantReceiveItemsUI;

    public GameObject[] prevent;
    public TextMeshProUGUI[] priceTexts;
    public TextMeshProUGUI[] priceFarmStallTexts;
    public void Start()
    {
        UpdatePrice();
        UpdatePriceFarmStall();
    }
    public void Sell()
    {
        currentState = MerchantState.NotForSale;
        Notification.Instance.ShowNotification("Đã bán với thương nhân!");
        Gold.Instance.AddGold(merchant.totalAmount);
        merchant.AddQuantityItemsSold();
        UpdateReceiveDataItem();
        UpdatePrevent();
        UpdateUI();
    }
    public void UpdateUI()
    {
        totalAmountText.text = merchant.totalAmount.ToString("N0", new CultureInfo("de-DE"));

        if (merchant.totalAmount > 0)
        {
            currentState = MerchantState.ReadyForSale;
        }
        else
        {
            currentState = MerchantState.NotForSale;
        }

        switch (currentState)
        {
            case MerchantState.NotForSale:
                buttonSell.interactable = false;
                break;
            case MerchantState.ReadyForSale:
                buttonSell.interactable = true;
                break;              
        }

    }

    public void UpdateReceiveDataItem()
    {
        for (int i = 0; i < receiveItems.Length; i++)
        {
            if (receiveItems[i] != null && merchantReceiveItemsUI[i] != null)
            {
                receiveItems[i].DestroyDataItem();
                merchantReceiveItemsUI[i].UpdateAllSlots();
            }
        }
    }
    public void UpdatePrevent()
    {
        for (int i = 0; i < merchant.salesLimit.Length; i++)
        {
            if (merchant.salesLimit[i] == merchant.quantityItemsSold[i])
            {
                prevent[i].SetActive(true);
            }
            else
            {
                prevent[i].SetActive(false);
            }
        }
    }
    public void UpdatePrice()
    {
        for (int i = 0; i < priceTexts.Length; i++)
        {
            if (priceTexts[i] != null)
            {
                priceTexts[i].text = (merchant.bonusSellPrice[i] + merchant.farmStall.sellPriceSpring[i]).ToString();
            }
        }
    }
    public void UpdatePriceFarmStall()
    {
        for (int i = 0; i < priceFarmStallTexts.Length; i++)
        {
            if (priceFarmStallTexts[i] != null)
            {
                priceFarmStallTexts[i].text = merchant.farmStall.sellPriceSpring[i].ToString();
            }
        }
    }
}
