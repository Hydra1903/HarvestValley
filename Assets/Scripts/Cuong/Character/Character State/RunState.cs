using UnityEngine;

public class RunState : ICharacterState
{
     
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("Run");
        CharacterSound.Instance.audioSourceCharacter.pitch = 1.2f;
        CharacterSound.Instance.audioSourceCharacter.Play();      
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.CameraController();
        characterStateMachine.PlayerMovement(6.5f);
        if (characterStateMachine.horizontal == 0 && characterStateMachine.vertical == 0)
        {
            characterStateMachine.ChangeState(characterStateMachine.idleState);
        }
        if ((characterStateMachine.horizontal != 0 || characterStateMachine.vertical != 0) && Input.GetKeyUp(KeyCode.LeftShift))
        {
            characterStateMachine.ChangeState(characterStateMachine.walkState);
        }
        if (characterStateMachine.mainUIScreen.staminaBar.value <= 0 )
        {
            characterStateMachine.ChangeState(characterStateMachine.walkState);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        CharacterSound.Instance.audioSourceCharacter.pitch = 1f;
        CharacterSound.Instance.audioSourceCharacter.Stop();
    }
}
