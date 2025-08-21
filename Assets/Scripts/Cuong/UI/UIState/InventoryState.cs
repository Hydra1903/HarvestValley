using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
public enum EInventoryState
{
    Inventory,
    Achievement,
    Unlock,
    Plant
}
public enum StatisticsTable
{
    On,
    Off
}
public class InventoryState : IUIState
{
    public EInventoryState currentInventoryState;

    public void Enter()
    {
        UIManager.Instance.ShowUI("MainInventory");
        UIManager.Instance.ShowUI("Panel");
        ResetUI();
        UIStateMachine.Instance.inventoryUI1.UpdateAllSlots();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.mainScreenState);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIStateMachine.Instance.ChangeState(UIStateMachine.Instance.pauseState);
        }
    }
    public void Exit()
    {
        UIManager.Instance.HideUI("MainInventory");
    }
    public void ChangeSubState(EInventoryState newState)
    {
        UIStateMachine.Instance.scrollViewAchievement.verticalNormalizedPosition = 1f;
        UIStateMachine.Instance.scrollViewUnlock.verticalNormalizedPosition = 1f;
        CheckHideUI();
        currentInventoryState = newState;
        CheckShowUI();
    }
    public void ResetUI()
    {
        if (currentInventoryState != EInventoryState.Inventory)
        {
            CheckHideUI();
            currentInventoryState = EInventoryState.Inventory;
            CheckShowUI();
        }

        UIStateMachine.Instance.btnInventory.interactable = false;
        UIStateMachine.Instance.btnAchievement.interactable = true;
        UIStateMachine.Instance.btnUnlock.interactable = true;
        UIStateMachine.Instance.btnPlant.interactable = true;

        UIStateMachine.Instance.scrollViewAchievement.verticalNormalizedPosition = 1f;
        UIStateMachine.Instance.scrollViewUnlock.verticalNormalizedPosition = 1f;
    }
    public void CheckShowUI()
    {
        switch (currentInventoryState)
        {
            case EInventoryState.Inventory:
                UIManager.Instance.ShowUI("Inventory1");
                break;
            case EInventoryState.Achievement:
                UIManager.Instance.ShowUI("Achievement");
                UIManager.Instance.ShowUI("StatisticsTable");
                break;
            case EInventoryState.Unlock:
                UIManager.Instance.ShowUI("Unlock");
                break;
            case EInventoryState.Plant:
                UIManager.Instance.ShowUI("Plant");
                break;
        }
    }
    public void CheckHideUI()
    {
        switch (currentInventoryState)
        {
            case EInventoryState.Inventory:
                UIManager.Instance.HideUI("Inventory1");
                break;
            case EInventoryState.Achievement:
                UIManager.Instance.HideUI("Achievement");
                UIManager.Instance.HideUI("StatisticsTable");
                break;
            case EInventoryState.Unlock:
                UIManager.Instance.HideUI("Unlock");
                break;
            case EInventoryState.Plant:
                UIManager.Instance.HideUI("Plant");
                break;
        }
    }
}
