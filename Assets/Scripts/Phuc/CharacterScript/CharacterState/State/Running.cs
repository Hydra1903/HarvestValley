using UnityEngine;

public class Running : MovementBaseState
{
    public override void EnterState(PlayerController movement)
    {
        movement.animator.Play("RunDeadline");
    }
    public override void UpdateState(PlayerController movement)
    {
        movement.Move(2.0f);
        if (movement.hzInput == 0 && movement.vInput == 0)
        {
            ExitState(movement, movement.IdleState);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ExitState(movement, movement.WalkState);
        }
       
        else
        {
          //  Debug.Log("Khong co trang thai");
        }
    }
    public override void ExitState(PlayerController movement, MovementBaseState state)
    {
     movement.SwitchState(state);
    }
}
