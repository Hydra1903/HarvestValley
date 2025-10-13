using UnityEngine;

public class FarmStallState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("FarmStall");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
        FarmStallUI.Instance.ResetUI();
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
        FarmStallUI.Instance.ReturnItem();
        UIManager.Instance.HideUI("FarmStall");
        UIManager.Instance.HideUI("Inventory");
    }
}
