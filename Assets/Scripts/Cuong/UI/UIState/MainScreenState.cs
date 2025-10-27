using UnityEngine;
public enum CalendarState
{
    On,
    Off
}
public class MainScreenState : IUIState
{
    public CalendarState currentCalendarState = CalendarState.Off;
    private HotBarUI hotBarUI;
    private HotBar hotBar;
    public void Enter()
    {
        UIManager.Instance.ShowUI("MainScreen");
        UIManager.Instance.HideUI("Panel");
        if (hotBarUI == null)
            hotBarUI = GameObject.FindFirstObjectByType<HotBarUI>();
        if (hotBar == null)
            hotBar = GameObject.FindFirstObjectByType<HotBar>();
        hotBar.UpdateData();
        hotBarUI.UpdateAllSlots();
        hotBarUI.UpdateCurrentItem(hotBarUI.currentHighlightIndex);
        UIStateMachine.Instance.inputCooldown = 1f;

        MusicBackground.Instance.audioSourceMusicDay.volume *= 2f;
        MusicBackground.Instance.audioSourceMusicNight.volume *= 2f;
        MainUIScreen.Instance.UpdateGold();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Update()
    {
        if (UIStateMachine.Instance.inputCooldown > 0)
        {
            UIStateMachine.Instance.inputCooldown -= Time.deltaTime;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.inventoryState);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentCalendarState == CalendarState.Off)
            {
                currentCalendarState = CalendarState.On;
            }
            else
            {
                currentCalendarState = CalendarState.Off;
            }
            ChangeCalendarState();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.pauseState);
        }      
        if (Input.GetKeyDown(KeyCode.M))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mapState);
        }
    }
    public void Exit()
    {
        if (currentCalendarState == CalendarState.On)
        {
            currentCalendarState = CalendarState.Off;
            ChangeCalendarState();
        }
        else
        {
            currentCalendarState = CalendarState.Off;
        }   
        UIManager.Instance.HideUI("MainScreen");

        if (WaterCan.Instance.currentState == EWaterCanState.On)
        {
            WaterCan.Instance.currentState = EWaterCanState.Off;
            WaterCan.Instance.CheckCurrentState();
        }
        if (ChangeMode.Instance.currentState == EChangeModeState.On)
        {
            ChangeMode.Instance.currentState = EChangeModeState.Off;
            ChangeMode.Instance.CheckCurrentState();
        }
        if (ChangeInteract.Instance.currentState == EChangeInteractState.On)
        {
            ChangeInteract.Instance.currentState = EChangeInteractState.Off;
            ChangeInteract.Instance.CheckCurrentState();
        }

        MusicBackground.Instance.audioSourceMusicDay.volume /= 2f;
        MusicBackground.Instance.audioSourceMusicNight.volume /= 2f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ChangeCalendarState()
    {

        if (currentCalendarState == CalendarState.Off)
        {
            UIManager.Instance.HideUI("Calendar");
            UIManager.Instance.ShowUI("IconAndTimeLine");
        }
        else if (currentCalendarState == CalendarState.On)
        {
            UIManager.Instance.ShowUI("Calendar");
            UIManager.Instance.HideUI("IconAndTimeLine");
        }
    }
}
