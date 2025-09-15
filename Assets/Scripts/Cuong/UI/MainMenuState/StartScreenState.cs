using UnityEngine;

public class StartScreenState : IUIState
{
    public void Enter()
    {
        MainMenuStateMachine.Instance.animatorCamera.Play("MoveOff");
        UIManager.Instance.ShowUI("StartScreen");
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
        UIManager.Instance.HideUI("StartScreen");
    }
}
