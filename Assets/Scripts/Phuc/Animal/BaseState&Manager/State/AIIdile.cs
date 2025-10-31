using UnityEngine;
using System.Collections;
public class AIIdile : AnimalMoveBaseState
{
    private bool hasDecided = false;
    private int randomResult;

    public override void EnterState(SimpleAI manager)
    {
        manager.animator.Play("Idle");
        manager.StartCoroutine(WaitAndDecide(manager));
        hasDecided = false;
    }
    public override void UpdateState(SimpleAI manager)
    {
        if (!hasDecided) return;
        //Bat su kien random tu 0 - 4
        switch (randomResult)
        {
            case 0:
                ExitState(manager, manager.turning); // Turning (Xoay theo huong den waypoint -> van dang bug)
                break;
            case 1:
                ExitState(manager, manager.eating);  // Eating (An co )
                break;
            case 2:
                ExitState(manager, manager.sitRelax); // Sit Relax (Ngoi nghi ngoi)
                break;
            case 3:
                ExitState(manager, manager.idle2); // Idle2 (idle thu 2 )
                break;
            case 4:
                ExitState(manager, manager.idle);// Main Idle (idle thu 1)
                break;
        }
    }
    public override void ExitState(SimpleAI manager, AnimalMoveBaseState nextState)
    {
        manager.SwitchState(nextState);
    }
    private IEnumerator WaitAndDecide(SimpleAI manager)
    {
        yield return new WaitForSeconds(Random.Range(6f, 10f));//thoi gian de random 
        randomResult = Random.Range(0, 5);// random tu 0 - 4 (0,1,2,3)
        hasDecided = true;
    }
}
