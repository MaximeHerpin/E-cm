using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QueueAndEat : StateMachineBehaviour
{

    NavMeshAgent agent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        Vector3 destination;
        Vector3 position = animator.transform.position;

        destination = FindQueue(position);
        agent.SetDestination(destination);
        animator.ResetTrigger("DestinationReached");
    }



    private Vector3 FindQueue(Vector3 position)
    {
        return GameObject.FindGameObjectWithTag("Queue").GetComponent<ClassRoom>().GetNextPosition();
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
