using UnityEngine;

public class BarnState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Barn");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
        UIStateMachine.Instance.inventoryUI2.UpdateAllSlots();
        UIStateMachine.Instance.inventoryUI2.UpdateGoldUI();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
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
        UIManager.Instance.HideUI("Barn");
        UIManager.Instance.HideUI("Inventory");
    }
}
