using UnityEngine;

public class SettingState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Setting");
        UIManager.Instance.ShowUI("Panel");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.pauseState);
        }
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("Setting");
    }
}
