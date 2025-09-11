
using UnityEngine;

public class IdleState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        CharacterStateMachine.Instance.animator.Play("Idle");
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.CameraController();
        if (characterStateMachine.horizontal != 0 || characterStateMachine.vertical != 0)
        {
            characterStateMachine.ChangeState(characterStateMachine.walkState);
        }    
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            characterStateMachine.ChangeState(characterStateMachine.hoeState);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            characterStateMachine.ChangeState(characterStateMachine.digHoleState);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            characterStateMachine.ChangeState(characterStateMachine.harvestLowState);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            characterStateMachine.ChangeState(characterStateMachine.harvestHighState);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            characterStateMachine.ChangeState(characterStateMachine.wateringState);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            characterStateMachine.ChangeState(characterStateMachine.mowingState);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {

    }
}
