using UnityEngine;
public enum EHouseDoorState
{
    Open,
    Close
}
public class HouseDoor : MonoBehaviour
{
    public Animator animator;
    public TriggerCanvas triggerCanvas;
    public EHouseDoorState currentState = EHouseDoorState.Close;
    public float timeCooldown = 1;
    void Update()
    {
        if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EHouseDoorState.Close && timeCooldown <= 0)
        {
            Open();
        }
        else if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EHouseDoorState.Open && timeCooldown <= 0)
        {
            Close();
        }
        if (timeCooldown > 0)
        {
            timeCooldown -= Time.deltaTime;
        }
    }
    public void Open()
    {
        ChangeState(currentState);
        animator.Play("Open");
        timeCooldown = 1f;
    }
    public void Close()
    {
        ChangeState(currentState);
        animator.Play("Close");
        timeCooldown = 1f;
    }
    public void ChangeState(EHouseDoorState currentState)
    {
        if (currentState == EHouseDoorState.Close)
        {
            this.currentState = EHouseDoorState.Open;
        }
        else if (currentState == EHouseDoorState.Open)
        {
            this.currentState = EHouseDoorState.Close;
        }
    }
}
