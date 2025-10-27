using UnityEngine;

public class SleepState : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("Sleeping");

        GameTime.Instance.PauseGame();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Update()
    {
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("Sleeping");
    }
}
