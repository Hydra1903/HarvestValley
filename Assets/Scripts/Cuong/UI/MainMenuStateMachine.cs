using UnityEngine;

public class MainMenuStateMachine : MonoBehaviour
{
    public static MainMenuStateMachine Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private IUIState currentState;
    public Animator animatorCamera;
    [HideInInspector] public StartScreenState startScreenState = new StartScreenState();
    [HideInInspector] public CharacterSelectionState characterSelectionState = new CharacterSelectionState();
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
        currentState = startScreenState;
    }
    void Update()
    {
        if (currentState != null)
            currentState.Update();
    }
}
