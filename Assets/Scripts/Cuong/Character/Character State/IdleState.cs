
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            characterStateMachine.ChangeState(characterStateMachine.hoeState);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            characterStateMachine.ChangeState(characterStateMachine.digHoleState);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            characterStateMachine.ChangeState(characterStateMachine.harvestLowState);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            characterStateMachine.ChangeState(characterStateMachine.harvestHighState);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            characterStateMachine.ChangeState(characterStateMachine.wateringState);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            characterStateMachine.ChangeState(characterStateMachine.mowingState);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {

    }
}
