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
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
