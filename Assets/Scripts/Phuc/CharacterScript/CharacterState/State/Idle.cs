using UnityEngine;

public class Idle : MovementBaseState
{
    public override void EnterState(PlayerController movement)
    {
        movement.animator.Play("idling");
    }
    public override void UpdateState(PlayerController movement)
    {
        if (movement.hzInput != 0 || movement.vInput != 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ExitState(movement, movement.RunState);
            }
            else
            {
                ExitState(movement, movement.WalkState);
            }
        }
     

    }
    public override void ExitState(PlayerController movement, MovementBaseState state)
    {
        movement.SwitchState(state);
    }      
}
