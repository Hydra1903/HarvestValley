using UnityEngine;

public class AnimalBarn1State : IUIState
{
    public void Enter()
    {
        UIManager.Instance.ShowUI("AnimalBarn1");
        UIManager.Instance.ShowUI("Inventory");
        UIManager.Instance.ShowUI("Panel");
        UIStateMachine.Instance.inputCooldown = 1f;
    }
    public void Update()
    {
        if (UIStateMachine.Instance.inputCooldown > 0)
        {
            UIStateMachine.Instance.inputCooldown -= Time.deltaTime;
            return;
        }
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
        UIManager.Instance.HideUI("AnimalBarn1");
        UIManager.Instance.HideUI("Inventory");
    }
}
