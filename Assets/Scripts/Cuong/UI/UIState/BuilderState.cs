using UnityEngine;

public class BuilderState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Builder");
        UIManager.Instance.ShowUI("Panel");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
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
        UIManager.Instance.HideUI("Builder");
    }
}
