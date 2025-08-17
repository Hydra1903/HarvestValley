using UnityEngine;
public enum CalendarState
{
    On,
    Off
}
public class MainScreenState : IUIState
{
    public CalendarState currentCalendarState = CalendarState.Off;
    public void Enter()
    {
        UIStateMachine.Instance.panelMainScreen.SetActive(true);
        currentCalendarState = CalendarState.Off;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.inventoryState);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeCalendarState();
        }
    }
    public void Exit()
    {
        UIStateMachine.Instance.panelMainScreen.SetActive(false);
    }
    public void ChangeCalendarState()
    {
        if (currentCalendarState == CalendarState.Off)
        {
            UIStateMachine.Instance.panelCalendar.SetActive(true);
            UIStateMachine.Instance.panelIconAndTimeLine.SetActive(false);
            currentCalendarState = CalendarState.On;
        }
        else if (currentCalendarState == CalendarState.On)
        {
            UIStateMachine.Instance.panelCalendar.SetActive(false);
            UIStateMachine.Instance.panelIconAndTimeLine.SetActive(true);
            currentCalendarState = CalendarState.Off;
        }
    }
}
