using UnityEngine;

public class StartScreenState : IUIState
{
    public void Enter()
    {   
        UIManager.Instance.ShowUI("StartScreen");
    }
    public void Update()
    {
        
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("StartScreen");
    }
}
