using UnityEngine;

public class StartScreenState : IUIState
{
    public void Enter()
    {   
        UIManager.Instance.ShowUI("StartScreen");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.settingMainMenuState);
        }
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("StartScreen");
    }
}
