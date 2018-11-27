using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FSM;

public class MandatoryMovement : Action {

    AgendaComponent ag;
    NavMeshAgent agent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(FSMComponent fsm)
    {
        ag = fsm.gameObject.GetComponent<AgendaComponent>();
        agent = fsm.gameObject.GetComponent<NavMeshAgent>();

        Vector3 destination = ag.currentEvent.classroom.GetComponent<ClassRoom>().GetNextPosition();
        agent.SetDestination(destination); // change agent destination
        agent.stoppingDistance = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(FSMComponent fsm)
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    fsm.SetTrigger("DestinationReached");
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(FSMComponent fsm)
    {

    }
}
