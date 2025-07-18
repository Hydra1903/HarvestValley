using UnityEngine;

public abstract class AnimalMoveBaseState
{
    public abstract void EnterState(SimpleAI StateMain);
    public abstract void UpdateState(SimpleAI StateMain);
    public abstract void ExitState(SimpleAI StateMain, AnimalMoveBaseState state);
}


