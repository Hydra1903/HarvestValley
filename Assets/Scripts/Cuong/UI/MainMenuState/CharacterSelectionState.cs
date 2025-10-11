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
        
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("CharacterSelection");
        MainMenuStateMachine.Instance.animatorCamera.Play("MoveOff");
    }
}
