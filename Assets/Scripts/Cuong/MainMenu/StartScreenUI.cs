using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    public Button buttonNewGame;
    public Button buttonBack;
    public Button buttonSetting;
    void Start()
    {
        buttonNewGame.onClick.AddListener(() => MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.characterSelectionState));
        buttonBack.onClick.AddListener(() => MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.startScreenState));
        buttonSetting.onClick.AddListener(() => MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.settingMainMenuState));
    }
    public void SwitchLeft()
    {
        CharacterSelection.Instance.ChangeCharacter(-1);
    }
    public void SwitchRight()
    {
        CharacterSelection.Instance.ChangeCharacter(1);
    }
}
