using UnityEngine;

public class Fall : MovementBaseState
{
    public override void EnterState(PlayerController movement)
    {
        movement.animator.Play("Falling");
    }
    public override void UpdateState(PlayerController movement)
    {
    
    }
    public override void ExitState(PlayerController movement, MovementBaseState state)
    {
        movement.SwitchState(state);
    }
}
