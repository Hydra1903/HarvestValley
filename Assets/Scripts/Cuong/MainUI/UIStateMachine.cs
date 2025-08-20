using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class UIStateMachine : MonoBehaviour
{
    public static UIStateMachine Instance;
    public InventoryUI inventoryUI;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private IUIState currentState;

    [HideInInspector] public MainScreenState mainScreenState = new MainScreenState();
    [HideInInspector] public InventoryState inventoryState = new InventoryState();
    [HideInInspector] public PauseState pauseState = new PauseState();
    [HideInInspector] public SettingState settingState = new SettingState();

    public Button btnInventory;
    public Button btnAchievement;
    public Button btnUnlock;
    public Button btnPlant;

    public ScrollRect scrollViewAchievement;
    public ScrollRect scrollViewUnlock;

    public Button btnSetting;
    public Button btnContinue;

    public Button btnDisplay;
    public Button btnSound;
    public Button btnControl;

    public ScrollRect scrollViewDisplay;
    public ScrollRect scrollViewSound;
    public ScrollRect scrollViewControl;

    public void ChangeState(IUIState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }

    void Start()
    {
        currentState = mainScreenState;
        btnInventory.onClick.AddListener(() => inventoryState.ChangeSubState(EInventoryState.Inventory));
        btnAchievement.onClick.AddListener(() => inventoryState.ChangeSubState(EInventoryState.Achievement));
        btnUnlock.onClick.AddListener(() => inventoryState.ChangeSubState(EInventoryState.Unlock));
        btnPlant.onClick.AddListener(() => inventoryState.ChangeSubState(EInventoryState.Plant));

        btnSetting.onClick.AddListener(() => ChangeState(settingState));

        btnDisplay.onClick.AddListener(() => settingState.ChangeSubState(ESettingState.Display));
        btnSound.onClick.AddListener(() => settingState.ChangeSubState(ESettingState.Sound));
        btnControl.onClick.AddListener(() => settingState.ChangeSubState(ESettingState.Control));

        btnContinue.onClick.AddListener(() => ChangeState(mainScreenState));
    }
    void Update()
    {
        if (currentState != null)
            currentState.Update();
    }
}
