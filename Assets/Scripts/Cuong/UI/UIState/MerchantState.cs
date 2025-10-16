using Unity.VisualScripting;
using UnityEngine;

public class MerchantState : IUIState
{
    public enum EMerchantState
    {
        BuyFarmProducts,
        SellSeeds      
    }
    public enum EBuyState
    {
        BuyCrops,
        BuyAnimalProducts
    }
    public EMerchantState currentMerchantState;
    public EBuyState currentBuyState;
    public void Enter()
    {
        
        UIManager.Instance.ShowUI("Merchant");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
        MerchantUI.Instance.ResetUI();
        UIStateMachine.Instance.inventoryUI2.UpdateAllSlots();
        UIStateMachine.Instance.inventoryUI2.UpdateGoldUI();
        UIStateMachine.Instance.inputCooldown = 1f;
    }
    public void Update()
    {
        if (UIStateMachine.Instance.inputCooldown > 0)
        {
            UIStateMachine.Instance.inputCooldown -= Time.deltaTime;
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mainScreenState);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.pauseState);
        }
    }
    public void Exit()
    {
        ReturnItem();
        UIManager.Instance.HideUI("Merchant");
        UIManager.Instance.HideUI("Inventory");
    }
    public void ChangeMerchantState(EMerchantState newState)
    {
        ReturnItem();
        UIStateMachine.Instance.inventoryUI2.UpdateAllSlots();
        CheckHideUIMerchantState();
        currentMerchantState = newState;
        CheckShowUIMerchantState();
    }
    public void ChangeBuyState(EBuyState newState)
    {
        ReturnItem();
        UIStateMachine.Instance.inventoryUI2.UpdateAllSlots();
        CheckHideUIBuyState();
        currentBuyState = newState;
        CheckShowUIBuyState();
    }
    public void CheckShowUIMerchantState()
    {
        switch (currentMerchantState)
        {
            case EMerchantState.SellSeeds:
                UIManager.Instance.ShowUI("SellSeeds");
                break;
            case EMerchantState.BuyFarmProducts:
                UIManager.Instance.ShowUI("BuyFarmProducts");
                break;
        }
    }
    public void CheckHideUIMerchantState()
    {
        switch (currentMerchantState)
        {
            case EMerchantState.SellSeeds:
                UIManager.Instance.HideUI("SellSeeds");
                break;
            case EMerchantState.BuyFarmProducts:
                UIManager.Instance.HideUI("BuyFarmProducts");
                break;
        }
    }
    public void CheckShowUIBuyState()
    {
        switch (currentBuyState)
        {
            case EBuyState.BuyCrops:
                UIManager.Instance.ShowUI("BuyCrops");
                break;
            case EBuyState.BuyAnimalProducts:
                UIManager.Instance.ShowUI("BuyAnimalProducts");
                break;
        }
    }
    public void CheckHideUIBuyState()
    {
        switch (currentBuyState)
        {
            case EBuyState.BuyCrops:
                UIManager.Instance.HideUI("BuyCrops");
                break;
            case EBuyState.BuyAnimalProducts:
                UIManager.Instance.HideUI("BuyAnimalProducts");
                break;
        }
    }
    public void ReturnItem()
    {
        if (currentBuyState == EBuyState.BuyCrops)
        {
            MerchantUI.Instance.ReturnItemCrops();
        }
        else if (currentBuyState == EBuyState.BuyAnimalProducts)
        {
            MerchantUI.Instance.ReturnItemAnimalProducts();
        }
    }
}
