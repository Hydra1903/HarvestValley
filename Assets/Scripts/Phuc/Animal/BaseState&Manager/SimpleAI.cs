using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
public class SimpleAI : MonoBehaviour
{
     public NavMeshAgent agent;
     public Animator animator;

    public Transform[] wanderPoints;

    private AnimalMoveBaseState currentState;

    // STATE
    public AIIdile idle = new AIIdile();
    public AIIdle2 idle2 = new AIIdle2();
    public AIWalking wander = new AIWalking();
    public AITurning turning = new AITurning();
    public AIEating eating = new AIEating();
    public AISitRelaxThenStandUp sitRelax = new AISitRelaxThenStandUp();
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        SwitchState(idle);
    }

    void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SwitchState(AnimalMoveBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }
}
