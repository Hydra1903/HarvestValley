using UnityEngine;
public enum EPenDoorState
{
    Open,
    Close
}
public class PenDoor : MonoBehaviour
{
    public Animator animator;
    public TriggerCanvas triggerCanvas;
    public EPenDoorState currentState = EPenDoorState.Close;
    public float timeCooldown = 1;
    void Update()
    {
        if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EPenDoorState.Close && timeCooldown <= 0)
        {
            Open();
        }
        else if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EPenDoorState.Open && timeCooldown <= 0)
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
    public void ChangeState(EPenDoorState currentState)
    {
        if (currentState == EPenDoorState.Close)
        {
            this.currentState = EPenDoorState.Open;
        }
        else if (currentState == EPenDoorState.Open)
        {
            this.currentState = EPenDoorState.Close;
        }
    }
}
