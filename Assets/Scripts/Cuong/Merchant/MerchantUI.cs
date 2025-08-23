using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using static MerchantState;

public enum EMerchantSaleState
{
    NotForSale,
    ReadyForSale
}
public class MerchantUI : MonoBehaviour
{
    public static MerchantUI Instance;
    public Merchant merchant;
    public TextMeshProUGUI totalAmountText;
    public Button buttonSell;
    public EMerchantSaleState currentState = EMerchantSaleState.NotForSale;

    public ReceiveItem[] receiveItems;
    public MerchantReceiveItemUI[] merchantReceiveItemsUI;

    public GameObject[] prevent;
    public TextMeshProUGUI[] priceTexts;
    public TextMeshProUGUI[] priceFarmStallTexts;

    public ScrollRect scrollViewSeed;
    public ScrollRect scrollViewPlant;
    public ScrollRect scrollViewAnimalProduct;
    public SeedShop seedShop;
    public SeedShopItemUI[] seedShopItemUI;

    public Button buttonBuyFarmProducts;
    public Button buttonSellSeeds;
    public Button buttonBuyCrops;
    public Button buttonBuyAnimalProducts;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void Start()
    {
        buttonBuyFarmProducts.onClick.AddListener(() => UIStateMachine.Instance.merchantState.ChangeMerchantState(EMerchantState.BuyFarmProducts));
        buttonSellSeeds.onClick.AddListener(() => UIStateMachine.Instance.merchantState.ChangeMerchantState(EMerchantState.SellSeeds));
        buttonBuyCrops.onClick.AddListener(() => UIStateMachine.Instance.merchantState.ChangeBuyState(EBuyState.BuyCrops));
        buttonBuyAnimalProducts.onClick.AddListener(() => UIStateMachine.Instance.merchantState.ChangeBuyState(EBuyState.BuyAnimalProducts));
    }
    public void Sell()
    {
        currentState = EMerchantSaleState.NotForSale;
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
            currentState = EMerchantSaleState.ReadyForSale;
        }
        else
        {
            currentState = EMerchantSaleState.NotForSale;
        }

        switch (currentState)
        {
            case EMerchantSaleState.NotForSale:
                buttonSell.interactable = false;
                break;
            case EMerchantSaleState.ReadyForSale:
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
                switch (Season.Instance.currentSeason)
                {
                    case SeasonState.Spring:
                        priceTexts[i].text = (merchant.bonusSellPrice[i] + merchant.farmStall.sellPriceSpring[i]).ToString();
                        break;
                    case SeasonState.Summer:
                        priceTexts[i].text = (merchant.bonusSellPrice[i] + merchant.farmStall.sellPriceSummer[i]).ToString();
                        break;
                    case SeasonState.Fall:
                        priceTexts[i].text = (merchant.bonusSellPrice[i] + merchant.farmStall.sellPriceFall[i]).ToString();
                        break;
                    case SeasonState.Winter:
                        priceTexts[i].text = (merchant.bonusSellPrice[i] + merchant.farmStall.sellPriceWinter[i]).ToString();
                        break;
                }
            }
        }
    }
    public void UpdatePriceFarmStall()
    {
        for (int i = 0; i < priceFarmStallTexts.Length; i++)
        {
            if (priceFarmStallTexts[i] != null)
            {
                switch (Season.Instance.currentSeason)
                {
                    case SeasonState.Spring:
                        priceFarmStallTexts[i].text = merchant.farmStall.sellPriceSpring[i].ToString();
                        break;
                    case SeasonState.Summer:
                        priceFarmStallTexts[i].text = merchant.farmStall.sellPriceSummer[i].ToString();
                        break;
                    case SeasonState.Fall:
                        priceFarmStallTexts[i].text = merchant.farmStall.sellPriceFall[i].ToString();
                        break;
                    case SeasonState.Winter:
                        priceFarmStallTexts[i].text = merchant.farmStall.sellPriceWinter[i].ToString();
                        break;
                }

            }
        }
    }
    public void ReturnItemCrops()
    {
        for (int i = 0; i < 28; i++)
        {
            if (receiveItems[i] != null && merchantReceiveItemsUI[i] != null)
            {
                receiveItems[i].ReturnItem();
                merchantReceiveItemsUI[i].UpdateAllSlots();
            }
        }
    }
    public void ReturnItemAnimalProducts()
    {
        for (int i = 28; i < 32; i++)
        {
            if (receiveItems[i] != null && merchantReceiveItemsUI[i] != null)
            {
                receiveItems[i].ReturnItem();
                merchantReceiveItemsUI[i].UpdateAllSlots();
            }
        }
    }
    public void ResetUI()
    {
        scrollViewSeed.verticalNormalizedPosition = 1f;
        scrollViewPlant.verticalNormalizedPosition = 1f;
        scrollViewAnimalProduct.verticalNormalizedPosition = 1f;   
        UpdatePrice();
        UpdatePriceFarmStall();
        for (int i = 0; i < seedShop.amount.Length; i++)
        {
            seedShop.amount[i] = 1;
            seedShopItemUI[i].CalculatePrice();
            seedShopItemUI[i].UpdateUI();
            seedShopItemUI[i].UpdateUnlock();
        }
        if (UIStateMachine.Instance.merchantState.currentMerchantState != EMerchantState.BuyFarmProducts)
        {
            UIStateMachine.Instance.merchantState.CheckHideUIMerchantState();
            UIStateMachine.Instance.merchantState.currentMerchantState = EMerchantState.BuyFarmProducts;
            UIStateMachine.Instance.merchantState.CheckShowUIMerchantState();
        }
        if (UIStateMachine.Instance.merchantState.currentBuyState != EBuyState.BuyCrops)
        {
            UIStateMachine.Instance.merchantState.CheckHideUIBuyState();
            UIStateMachine.Instance.merchantState.currentBuyState = EBuyState.BuyCrops;
            UIStateMachine.Instance.merchantState.CheckShowUIBuyState();
        }
        buttonBuyFarmProducts.interactable = false;
        buttonSellSeeds.interactable = true;
        buttonBuyCrops.interactable = false;
        buttonBuyAnimalProducts.interactable= true;
    }
}
