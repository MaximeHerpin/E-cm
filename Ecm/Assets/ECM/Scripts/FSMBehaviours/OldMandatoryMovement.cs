using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OldMandatoryMovement : StateMachineBehaviour {

    AgendaComponent ag;
    NavMeshAgent agent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ag = animator.gameObject.GetComponent<AgendaComponent>();
        agent = animator.gameObject.GetComponent<NavMeshAgent>();

        Vector3 destination = ag.currentEvent.classroom.GetComponent<ClassRoom>().GetNextPosition();
        agent.SetDestination(destination); // change agent destination
        agent.stoppingDistance = 0;
        animator.ResetTrigger("DestinationReached");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetTrigger("DestinationReached");
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
