using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    public Button buttonNewGame;
    void Start()
    {
        buttonNewGame.onClick.AddListener(() => MainMenuStateMachine.Instance.ChangeState(MainMenuStateMachine.Instance.characterSelectionState));
    } 
    void Update()
    {
        
    }
}
