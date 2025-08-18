using UnityEngine;
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
    
    public Button btnInventory;
    public Button btnAchievement;
    public Button btnUnlock;
    public Button btnPlant;

    public ScrollRect scrollViewAchievement;
    public ScrollRect scrollViewUnlock;

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
    }
    void Update()
    {
        if (currentState != null)
            currentState.Update();
    }
}
