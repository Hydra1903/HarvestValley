using UnityEngine;
public class SettingMainMenuState : IUIState
{
    public ESettingState currentSettingState;
    public void Enter()
    {
        UIManager.Instance.ShowUI("Setting");
        UIManager.Instance.ShowUI("Panel");
        ResetUI();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.startScreenState);
        }
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("Setting");
    }
    public void ChangeSubState(ESettingState newState)
    {
        ResetScrollView();
        CheckHideUI();
        currentSettingState = newState;
        CheckShowUI();
    }
    public void ResetUI()
    {
        if (currentSettingState != ESettingState.Display)
        {
            CheckHideUI();
            currentSettingState = ESettingState.Display;
            CheckShowUI();
        }
        ResetScrollView();
        SettingMainMemu.Instance.btnDisplay.interactable = false;
        SettingMainMemu.Instance.btnSound.interactable = true;
        SettingMainMemu.Instance.btnControl.interactable = true;
    }
    public void ResetScrollView()
    {
        SettingMainMemu.Instance.scrollViewDisplay.verticalNormalizedPosition = 1f;
        SettingMainMemu.Instance.scrollViewSound.verticalNormalizedPosition = 1f;
        SettingMainMemu.Instance.scrollViewControl.verticalNormalizedPosition = 1f;
    }
    public void CheckShowUI()
    {
        switch (currentSettingState)
        {
            case ESettingState.Display:
                UIManager.Instance.ShowUI("Display");
                break;
            case ESettingState.Sound:
                UIManager.Instance.ShowUI("Sound");
                break;
            case ESettingState.Control:
                UIManager.Instance.ShowUI("Control");
                break;
        }
    }
    public void CheckHideUI()
    {
        switch (currentSettingState)
        {
            case ESettingState.Display:
                UIManager.Instance.HideUI("Display");
                break;
            case ESettingState.Sound:
                UIManager.Instance.HideUI("Sound");
                break;
            case ESettingState.Control:
                UIManager.Instance.HideUI("Control");
                break;
        }
    }
}
