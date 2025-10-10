using UnityEngine;
using System.Collections;

public class AITurning : AnimalMoveBaseState
{
    private bool finishedTurning = false;
    private Quaternion targetRotation;
    //testing xoay 90... -> no van con loi~ 
    public override void EnterState(SimpleAI manager)
    {
        finishedTurning = false;

        if (manager.wanderPoints.Length == 0)
        {
            ExitState(manager, manager.idle);
            return;
        }
        Transform target = manager.wanderPoints[Random.Range(0, manager.wanderPoints.Length)];
        Vector3 direction = (target.position - manager.transform.position).normalized;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
       
        float angle = Vector3.SignedAngle(manager.transform.forward, direction, Vector3.up);
        if (angle < -10f)
        {
            manager.animator.Play("turn_90_L");
        }
        else if (angle > 10f)
        {
            manager.animator.Play("turn_90_R");
        }
        else
        {
            // Không c?n xoay
            finishedTurning = true;
            ExitState(manager, manager.wander);
            return;
        }
        manager.StartCoroutine(TurnToTarget(manager, targetRotation));
    }
    public override void UpdateState(SimpleAI manager)
    {
        if (finishedTurning)
        {
            ExitState(manager, manager.wander);
        }
    }
    public override void ExitState(SimpleAI manager, AnimalMoveBaseState nextState)
    {
        manager.SwitchState(nextState);
    }
    private IEnumerator TurnToTarget(SimpleAI manager, Quaternion targetRot)
    {
        float rotateSpeed = 3f;
        float turnDuration = 1.14f; 
        float timer = 0f;
        while (Quaternion.Angle(manager.transform.rotation, targetRot) > 1f)
        {
            manager.transform.rotation = Quaternion.Slerp(manager.transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            timer += Time.deltaTime;
            if (timer > turnDuration) break;
            yield return null;
        }
        manager.transform.rotation = targetRot;
        yield return new WaitForSeconds(0.1f);
        finishedTurning = true;
    }
}
