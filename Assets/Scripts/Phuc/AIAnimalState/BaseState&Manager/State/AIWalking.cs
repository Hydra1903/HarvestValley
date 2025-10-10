using UnityEngine;
using UnityEngine.AI;

public class AIWalking : AnimalMoveBaseState
{
    private bool hasArrived = false;

    public override void EnterState(SimpleAI manager)
    {
        hasArrived = false;
        if (manager.wanderPoints.Length == 0)
        {
            manager.SwitchState(manager.idle);
            return;
        }
        Transform point = manager.wanderPoints[Random.Range(0, manager.wanderPoints.Length)];
        manager.agent.isStopped = false;
        manager.agent.SetDestination(point.position);
        manager.animator.Play("Walk");
    }
    public override void UpdateState(SimpleAI manager)
    {
        if (!hasArrived && !manager.agent.pathPending && manager.agent.remainingDistance < 0.5f)
        {
            hasArrived = true;
            ExitState(manager, manager.idle);
        }
    }
    public override void ExitState(SimpleAI manager, AnimalMoveBaseState nextState)
    {
        manager.agent.isStopped = true;
        manager.SwitchState(nextState);
    }
}
