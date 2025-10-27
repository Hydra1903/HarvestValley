using UnityEngine;

public class WateringState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("Watering");
        if (CharacterStateMachine.Instance.currentCharacter == ECharacter.Rin)
        {
            characterStateMachine.animator.speed = 1.2f;
        }
        UIManager.Instance.ShowUI("ActionBar");
        CameraSwitcher.Instance.SwitchToActionView();
        CharacterStateMachine.Instance.mainUIScreen.ResetBar();
        SoundEffects.Instance.PlaySound_Watering();
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value < 1)
        {
            CharacterStateMachine.Instance.mainUIScreen.ActionTime(5f);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.speed = 1f;
        CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.SwitchToMainView());
        UIManager.Instance.HideUI("ActionBar");
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value >= 1f)
        {
            WaterCan.Instance.ConsumeWater();
            characterStateMachine.soilManager.TryWaterAt(characterStateMachine.farmInput.gridPos);
        }
    }
}
