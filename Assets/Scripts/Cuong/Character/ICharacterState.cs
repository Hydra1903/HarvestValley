public interface ICharacterState
{
    void Enter(CharacterStateMachine characterStateMachine);
    void Update(CharacterStateMachine characterStateMachine);
    void Exit(CharacterStateMachine characterStateMachine);
}
