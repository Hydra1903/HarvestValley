using UnityEngine;
public enum EBarnDoorState
{
    Open,
    Close

}
public class BarnDoor : MonoBehaviour
{
    public Animator animator;
    public TriggerCanvas triggerCanvas;
    public EBarnDoorState currentState = EBarnDoorState.Close;
    public float timeCooldown = 1;
    private void Update()
    {
        if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EBarnDoorState.Close && timeCooldown <= 0)
        {
            Open();
        }
        else if (triggerCanvas.isPlayerNearby && Input.GetKeyDown(KeyCode.E) && currentState == EBarnDoorState.Open && timeCooldown <= 0)
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
    public void ChangeState(EBarnDoorState currentState)
    {
        if (currentState == EBarnDoorState.Close)
        {
            this.currentState = EBarnDoorState.Open;
        }
        else if (currentState == EBarnDoorState.Open)
        {
            this.currentState = EBarnDoorState.Close;
        }
    }
}
