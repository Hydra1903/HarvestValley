using UnityEngine;

public class FarmStallState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("FarmStall");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
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
        UIManager.Instance.HideUI("FarmStall");
        UIManager.Instance.HideUI("Inventory");
    }
}
