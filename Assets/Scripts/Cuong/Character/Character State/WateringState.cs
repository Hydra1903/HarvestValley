using UnityEngine;

public class WateringState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("Watering");
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
