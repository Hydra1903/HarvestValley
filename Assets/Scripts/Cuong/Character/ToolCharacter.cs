using UnityEngine;

public class ToolCharacter : MonoBehaviour
{
    public static ToolCharacter Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public GameObject Hoe;
    public GameObject WateringCan;
    public GameObject Sickle;

    public void SetToolCharacter()
    {
        if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.hoeState)
        {
            HideTool();
            Hoe.SetActive(true);
        }
        else if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.digHoleState)
        {
            HideTool();
            Hoe.SetActive(true);
        }
        else if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.wateringState)
        {
            HideTool();
            WateringCan.SetActive(true);
        }
        else if (CharacterStateMachine.Instance.currentState == CharacterStateMachine.Instance.mowingState)
        {
            HideTool();
            Sickle.SetActive(true);
        }
        else
        {
            HideTool();
        }
    }
    public void HideTool()
    {
        Hoe.SetActive(false);
        WateringCan.SetActive(false);
        Sickle.SetActive(false);
    }
}
