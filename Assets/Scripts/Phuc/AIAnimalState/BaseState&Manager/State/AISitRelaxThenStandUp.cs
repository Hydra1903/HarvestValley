using UnityEngine;
using System.Collections;

public class AISitRelaxThenStandUp : AnimalMoveBaseState
{
    public override void EnterState(SimpleAI manager)
    {
        manager.animator.Play("stand_to_sit");
        manager.StartCoroutine(SitAndStand(manager));
    }

    public override void UpdateState(SimpleAI manager) { }

    public override void ExitState(SimpleAI manager, AnimalMoveBaseState nextState)
    {
        manager.SwitchState(nextState);
    }

    private IEnumerator SitAndStand(SimpleAI manager)
    {
        yield return new WaitForSeconds(10f); 
        manager.animator.Play("sit_to_stand"); 
        yield return new WaitForSeconds(3f);
        ExitState(manager, manager.idle);
    }
}