using UnityEngine;

public class PauseState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Pause");
        UIManager.Instance.ShowUI("Panel");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mainScreenState);
        }
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("Pause");
    }
}
