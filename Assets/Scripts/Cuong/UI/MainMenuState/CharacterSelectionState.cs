using UnityEngine;

public class CharacterSelectionState : IUIState
{
    public void Enter()
    {
        MainMenuStateMachine.Instance.animatorCamera.Play("MoveOn");
        UIManager.Instance.ShowUI("CharacterSelection");
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
        UIManager.Instance.HideUI("CharacterSelection");
    }
}
