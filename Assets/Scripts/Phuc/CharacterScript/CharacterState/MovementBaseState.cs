using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(PlayerController movement);
    public abstract void UpdateState(PlayerController movement);
    public abstract void ExitState(PlayerController movement, MovementBaseState state);
}