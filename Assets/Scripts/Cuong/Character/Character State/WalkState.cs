using UnityEngine;

public class WalkState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        CharacterStateMachine.Instance.animator.Play("Walk");
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.CameraController();
        characterStateMachine.PlayerMovement(4f);
        if (characterStateMachine.horizontal == 0 && characterStateMachine.vertical == 0)
        {
            characterStateMachine.ChangeState(characterStateMachine.idleState);
        }
        if ((characterStateMachine.horizontal != 0 || characterStateMachine.vertical != 0) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterStateMachine.ChangeState(characterStateMachine.runState);
        }

    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {

    }
}
