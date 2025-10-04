using UnityEngine;

public class MowingState : ICharacterState
{
    public void Enter(CharacterStateMachine characterStateMachine)
    {
        CharacterStateMachine.Instance.animator.Play("Mowing");
        UIManager.Instance.ShowUI("ActionBar");
        CameraSwitcher.Instance.SwitchToActionView();
        CharacterStateMachine.Instance.mainUIScreen.ResetBar();
    }
    public void Update(CharacterStateMachine characterStateMachine)
    {
        if (CharacterStateMachine.Instance.mainUIScreen.actionBar.value < 1)
        {
            CharacterStateMachine.Instance.mainUIScreen.ActionTime(6.05f);
        }
    }
    public void Exit(CharacterStateMachine characterStateMachine)
    {
        CameraSwitcher.Instance.StartCoroutine(CameraSwitcher.Instance.SwitchToMainView());
        UIManager.Instance.HideUI("ActionBar");
    }
}
