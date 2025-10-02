using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public enum FarmStallState
{
    NotForSale,   
    Selling,     
    ReadyToCollect  
}
public class FarmStallUI : MonoBehaviour
{
    public FarmStall farmStall;
    public TextMeshProUGUI totalAmountText;
    public TextMeshProUGUI statusText;
    public Button buttonSell;
    public Button buttonCollect;
    public GameObject prevent;
    public FarmStallState currentState = FarmStallState.NotForSale;

    public TextMeshProUGUI[] priceTexts;
    public ReceiveItem[] receiveItems;
    public ReceiveItemUI[] receiveItemsUI;

    public void Start()
    {
        UpdatePrice();
    }
    public void Sell()
    {
        currentState = FarmStallState.Selling;
        Notification.Instance.ShowNotification("Quay trở lại vào ngày mai!");
        UpdateUI();
    }
    public void Collect()
    {
        currentState = FarmStallState.NotForSale;
        Gold.Instance.AddGold(farmStall.totalAmount);
        farmStall.totalAmount = 0;
        UpdateReceiveDataItem();
        UpdateUI();
    }
    public void UpdateUI()
    {
        totalAmountText.text = farmStall.totalAmount.ToString("N0", new CultureInfo("de-DE"));

        if (farmStall.totalAmount > 0 && currentState == FarmStallState.NotForSale)
        {
            buttonSell.interactable = true;
        }
        else
        {
            buttonSell.interactable = false;
        }

        switch (currentState)
        {
            case FarmStallState.NotForSale:
                buttonCollect.interactable = false;
                prevent.SetActive(false);
                statusText.text = "Chưa có gì để bán!";
                break;
            case FarmStallState.Selling:
                buttonCollect.interactable = true;
                prevent.SetActive(true);
                statusText.text = "Đang bán!";
                break;
            case FarmStallState.ReadyToCollect:
                statusText.text = "Có thể nhận!";
                break;
        }
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

    public void UpdatePrice()
    {
        for (int i = 0; i < priceTexts.Length; i++)
        {
            if (priceTexts[i] != null)
            {
                priceTexts[i].text = farmStall.sellPriceSpring[i].ToString();
            }
        }
    }
}
