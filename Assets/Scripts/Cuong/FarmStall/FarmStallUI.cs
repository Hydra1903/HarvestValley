using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public FarmStallState currentState = FarmStallState.NotForSale;

    public void Sell()
    {
        currentState = FarmStallState.Selling;
        buttonSell.interactable = false;
        UpdateUI();
    }
    public void Collect()
    {
        currentState = FarmStallState.NotForSale;
        buttonCollect.interactable = false;
        UpdateUI();
    }
    public void UpdateUI()
    {
        totalAmountText.text = farmStall.totalAmount.ToString();
        switch (currentState)
        {
            case FarmStallState.NotForSale:
                statusText.text = "Chưa có gì để bán!";
                break;
            case FarmStallState.Selling:
                statusText.text = "Đang bán!";
                break;
            case FarmStallState.ReadyToCollect:
                statusText.text = "Có thể nhận!";
                break;
        }
    }
}
