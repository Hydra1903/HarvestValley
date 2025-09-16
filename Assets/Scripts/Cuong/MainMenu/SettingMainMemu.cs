using UnityEngine;
using UnityEngine.UI;

public class SettingMainMemu : MonoBehaviour
{
    public static SettingMainMemu Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public Button btnDisplay;
    public Button btnSound;
    public Button btnControl;
    public ScrollRect scrollViewDisplay;
    public ScrollRect scrollViewSound;
    public ScrollRect scrollViewControl;
    void Start()
    {
        btnDisplay.onClick.AddListener(() => MainMenuStateMachine.Instance.settingMainMenuState.ChangeSubState(ESettingState.Display));
        btnSound.onClick.AddListener(() => MainMenuStateMachine.Instance.settingMainMenuState.ChangeSubState(ESettingState.Sound));
        btnControl.onClick.AddListener(() => MainMenuStateMachine.Instance.settingMainMenuState.ChangeSubState(ESettingState.Control));
    }


    void Update()
    {
        
    }
}
