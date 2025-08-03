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

    public void Sell()
    {
        currentState = MerchantState.NotForSale;
        Notification.Instance.ShowNotification("Đã bán với thương nhân!");
        UpdateReceiveDataItem();
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
}
