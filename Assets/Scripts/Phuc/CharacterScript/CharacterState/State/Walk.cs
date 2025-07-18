using UnityEngine;

public class Walk : MovementBaseState
{
    public override void EnterState(PlayerController movement)
    { 
        movement.animator.Play("RunFW"); 
    }
    public override void UpdateState(PlayerController movement)
    {
        movement.Move(1);
        if (movement.hzInput == 0 && movement.vInput == 0)
        {
            ExitState(movement, movement.IdleState);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ExitState(movement, movement.RunState);
        }
    }
    public override void ExitState(PlayerController movement, MovementBaseState state)
    {
        movement.SwitchState(state);
    }

}
