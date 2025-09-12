using UnityEngine;

public class MowingState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        CharacterStateMachine.Instance.animator.Play("Mowing");
        CameraSwitcher.Instance.SwitchToActionView();
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        

    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.SwitchToMainView());
    }
}
