using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public enum EFarmStallState
{
    NotForSale,   
    Selling,     
    ReadyToCollect  
}
public class FarmStallUI : MonoBehaviour
{
    public static FarmStallUI Instance;
    public FarmStall farmStall;
    public TextMeshProUGUI totalAmountText;
    public TextMeshProUGUI statusText;
    public Button buttonSell;
    public Button buttonCollect;
    public GameObject prevent;
    public EFarmStallState currentState = EFarmStallState.NotForSale;

    public TextMeshProUGUI[] priceTexts;
    public ReceiveItem[] receiveItems;
    public ReceiveItemUI[] receiveItemsUI;

    public GameObject[] highlightSeason;
    public GameObject[] arrowIncrease;
    public GameObject[] arrowDecrease;
    public ScrollRect scrollViewFarmStall;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void Start()
    {
        UpdatePrice();
        UpdateUI();
    }
    public void Sell()
    {
        currentState = EFarmStallState.Selling;
        Notification.Instance.ShowNotification("Quay trở lại vào ngày mai!");
        UpdateUI();
    }
    public void Collect()
    {
        currentState = EFarmStallState.NotForSale;
        Gold.Instance.AddGold(farmStall.totalAmount);
        farmStall.totalAmount = 0;
        UpdateReceiveDataItem();
        UpdateUI();
    }
    public void UpdateUI()
    {
        totalAmountText.text = farmStall.totalAmount.ToString("N0", new CultureInfo("de-DE"));

        if (farmStall.totalAmount > 0 && currentState == EFarmStallState.NotForSale)
        {
            buttonSell.interactable = true;
        }
        else
        {
            buttonSell.interactable = false;
        }
        switch (currentState)
        {
            case EFarmStallState.NotForSale:
                buttonCollect.interactable = false;
                prevent.SetActive(false);
                statusText.text = "Chưa có gì để bán!";
                break;
            case EFarmStallState.Selling:
                prevent.SetActive(true);
                statusText.text = "Đang bán!";
                break;
            case EFarmStallState.ReadyToCollect:
                statusText.text = "Có thể nhận!";
                break;
        }
    }

    public void CanCollect()
    {
        buttonCollect.interactable = true;
        currentState = EFarmStallState.ReadyToCollect;
        UpdateUI();
    }
    public void UpdateReceiveDataItem()
    {
        for (int i = 0; i < receiveItems.Length; i++)
        {
            if (receiveItems[i] != null && receiveItemsUI[i] != null)
            {
                receiveItems[i].DestroyDataItem();
                receiveItemsUI[i].UpdateAllSlots();
            }
        }
    }
    public void ReturnItem()
    {
        if (currentState == EFarmStallState.NotForSale)
        {
            for (int i = 0; i < receiveItems.Length; i++)
            {
                if (receiveItems[i] != null && receiveItemsUI[i] != null)
                {
                    receiveItems[i].ReturnItem();
                    receiveItemsUI[i].UpdateAllSlots();
                }
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
                        priceTexts[i].text = farmStall.sellPriceSpring[i].ToString();
                        break;
                    case SeasonState.Summer:
                        priceTexts[i].text = farmStall.sellPriceSummer[i].ToString();
                        break;
                    case SeasonState.Fall:
                        priceTexts[i].text = farmStall.sellPriceFall[i].ToString();
                        break;
                    case SeasonState.Winter:
                        priceTexts[i].text = farmStall.sellPriceWinter[i].ToString();
                        break;
                }
            }
        }
    }

    public void SetCurrentSeasonInFarmStall()
    {
        for (int i = 0; i < highlightSeason.Length; i++)
            highlightSeason[i].SetActive(false);

        switch (Season.Instance.currentSeason)
        {
            case SeasonState.Spring:
                highlightSeason[0].SetActive(true);
                break;
            case SeasonState.Summer:
                highlightSeason[1].SetActive(true);
                break;
            case SeasonState.Fall:
                highlightSeason[2].SetActive(true);
                break;
            case SeasonState.Winter:
                highlightSeason[3].SetActive(true);
                break;
        }
    }
    public void UpdateArrow()
    {
        for (int i = 0; i < arrowIncrease.Length; i++)
        {
            arrowIncrease[i].SetActive(false);
            arrowDecrease[i].SetActive(false);
            switch (Season.Instance.currentSeason)
            {
                case SeasonState.Spring:
                    if (!farmStall.canGrowInSpring[i])
                    { arrowIncrease[i].SetActive(true); }
                    else
                    { arrowDecrease[i].SetActive(true); }
                    break;
                case SeasonState.Summer:
                    if (!farmStall.canGrowInSummer[i])
                    { arrowIncrease[i].SetActive(true); }
                    else
                    { arrowDecrease[i].SetActive(true); }
                    break;
                case SeasonState.Fall:
                    if (!farmStall.canGrowInFall[i])
                    { arrowIncrease[i].SetActive(true); }
                    else
                    { arrowDecrease[i].SetActive(true); }
                    break;
                case SeasonState.Winter:
                    if (!farmStall.canGrowInWinter[i])
                    { arrowIncrease[i].SetActive(true); }
                    else
                    { arrowDecrease[i].SetActive(true); }
                    break;
            }
        }
    }
    public void ResetUI()
    {
        scrollViewFarmStall.verticalNormalizedPosition = 1f;
        UpdatePrice();
        SetCurrentSeasonInFarmStall();
        UpdateArrow();
    }

}
