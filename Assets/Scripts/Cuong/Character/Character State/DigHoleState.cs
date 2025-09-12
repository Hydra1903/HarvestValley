public class DigHoleState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("DigHole");
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
