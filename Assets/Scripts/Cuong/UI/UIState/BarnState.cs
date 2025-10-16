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
        UIManager.Instance.HideUI("Barn");
        UIManager.Instance.HideUI("Inventory");
    }
}
