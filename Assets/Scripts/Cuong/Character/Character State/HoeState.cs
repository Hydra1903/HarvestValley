
using UnityEditor.EditorTools;
using UnityEngine;

public class HoeState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.Play("Hoe");
        if (CharacterSelection.currentCharacter == ECharacter.Rin)
        {
            characterStateMachine.animator.speed = 1.2f;
        }
        UIManager.Instance.ShowUI("ActionBar");
        CameraSwitcher.Instance.SwitchToActionView();
        CharacterStateMachine.Instance.mainUIScreen.ResetBar();
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value < 1)
        {
            CharacterStateMachine.Instance.mainUIScreen.ActionTime(6.25f);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        characterStateMachine.animator.speed = 1f;
        CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.SwitchToMainView());
        UIManager.Instance.HideUI("ActionBar");
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value >= 1f)
        {
              characterStateMachine.soilManager.HoeAt(characterStateMachine.farmInput.gridPos, characterStateMachine.farmInput.tool);
        }
    }
}
