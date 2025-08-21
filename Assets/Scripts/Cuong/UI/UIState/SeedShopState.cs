using UnityEngine;

public class SeedShopState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("SeedShop");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
        SeedShopUI.Instance.ResetUI();
        UIStateMachine.Instance.inventoryUI2.UpdateAllSlots();
        UIStateMachine.Instance.inventoryUI2.UpdateGoldUI();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
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
        UIManager.Instance.HideUI("SeedShop");
        UIManager.Instance.HideUI("Inventory");
    }
}
