using UnityEngine;
using System.Collections;

public class AIEating : AnimalMoveBaseState
{
    public override void EnterState(SimpleAI manager)
    {
        manager.animator.Play("Eating");
        manager.StartCoroutine(FinishEating(manager));
    }

    public override void UpdateState(SimpleAI manager)
    {
        
    }

    public override void ExitState(SimpleAI manager, AnimalMoveBaseState nextState)
    {
        manager.SwitchState(nextState);
    }

    private IEnumerator FinishEating(SimpleAI manager)
    {
        yield return new WaitForSeconds(6f); 
        ExitState(manager, manager.idle);
    }
}
