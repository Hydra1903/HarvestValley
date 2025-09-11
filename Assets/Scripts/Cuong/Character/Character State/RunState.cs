

using UnityEngine;

public class RunState : ICharacterState
{
     
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("Run");
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.CameraController();
        characterStateMachine.PlayerMovement(6f);
        if (characterStateMachine.horizontal == 0 && characterStateMachine.vertical == 0)
        {
            characterStateMachine.ChangeState(characterStateMachine.idleState);
        }
        if ((characterStateMachine.horizontal != 0 || characterStateMachine.vertical != 0) && Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterStateMachine.ChangeState(characterStateMachine.walkState);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {

    }
}
