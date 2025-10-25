using UnityEngine;
public enum EBedState
{
    Awake,
    Asleep
}
public class Bed : MonoBehaviour
{
    public TriggerCanvas triggerCanvas;
    public EBedState currentState;
    void Start()
    {
        
    }

    void Update()
    {
        if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EBedState.Awake)
        {
            Sleep();
        }
    }
    public void Sleep()
    {
        GameTime.Instance.NextDay();
        currentState = EBedState.Asleep;
    }   
}
