using UnityEngine;
public enum ESettingState
{
    Display,
    Sound,
    Control
}
public class SettingState : IUIState
{
    public ESettingState currentSettingState;
    public void Enter()
    {
        UIManager.Instance.ShowUI("Setting");
        UIManager.Instance.ShowUI("Panel");
        UIStateMachine.Instance.btnSetting.interactable = false;
        ResetUI();
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
        UIStateMachine.Instance.btnSetting.interactable = true;
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
        UIStateMachine.Instance.btnDisplay.interactable = false;
        UIStateMachine.Instance.btnSound.interactable = true;
        UIStateMachine.Instance.btnControl.interactable = true;
    }
    public void ResetScrollView()
    {
        UIStateMachine.Instance.scrollViewDisplay.verticalNormalizedPosition = 1f;
        UIStateMachine.Instance.scrollViewSound.verticalNormalizedPosition = 1f;
        UIStateMachine.Instance.scrollViewControl.verticalNormalizedPosition = 1f;
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
