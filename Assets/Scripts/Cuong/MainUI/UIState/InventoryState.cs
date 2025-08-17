using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
public enum EInventoryState
{
    Inventory,
    Achievement,
    Unlock,
    Plant
}
public class InventoryState : IUIState
{
    public EInventoryState currentInventoryState;
    public void Enter()
    {
        UIStateMachine.Instance.panelMainInventory.SetActive(true);
        ChangeSubState(EInventoryState.Inventory);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mainScreenState);
        }
    }
    public void Exit()
    {
        UIStateMachine.Instance.panelMainInventory.SetActive(false);
    }
    public void ChangeSubState(EInventoryState newState)
    {
        currentInventoryState = newState;
        UIStateMachine.Instance.panelInventory.SetActive(newState == EInventoryState.Inventory);
        UIStateMachine.Instance.panelAchievement.SetActive(newState == EInventoryState.Achievement);
        UIStateMachine.Instance.panelUnlock.SetActive(newState == EInventoryState.Unlock);
        UIStateMachine.Instance.panelPlant.SetActive(newState == EInventoryState.Plant);
    }
}
