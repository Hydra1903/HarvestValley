using UnityEngine;

public class MapState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Map");
        UIManager.Instance.ShowUI("Panel");
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
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
        UIManager.Instance.HideUI("Map");
    }
}
