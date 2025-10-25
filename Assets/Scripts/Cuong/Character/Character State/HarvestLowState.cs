public class HarvestLowState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("HarvestHigh");
        if (CharacterStateMachine.Instance.currentCharacter == ECharacter.Rin)
        {
            characterStateMachine.animator.speed = 1.2f;
        }
        UIManager.Instance.ShowUI("ActionBar");
        //CameraSwitcher.Instance.SwitchToActionView();
        CharacterStateMachine.Instance.mainUIScreen.ResetBar();
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value < 1)
        {
            CharacterStateMachine.Instance.mainUIScreen.ActionTime(1.06f);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        //CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.SwitchToMainView());
        characterStateMachine.animator.speed = 1f;
        UIManager.Instance.HideUI("ActionBar");
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value >= 1f)
        {
            characterStateMachine.plantManager.TryHarvest(characterStateMachine.farmInput.gridPos);
        }
    }
}
