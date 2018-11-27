using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FSM;

public class MovementToNeed : Action {

    int need;
    NavMeshAgent agent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(FSMComponent fsm)
    {
        need = (int) fsm.GetData();
        //need = 0;
        agent = fsm.gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 3;
        Vector3 destination;
        Vector3 position = fsm.transform.position;
        switch (need)
        {
            case 0:
                destination = FindFood(position);
                break;
            case 1:
                destination = FindToilet(position);
                break;
            case 2:
                destination = FindCoffee(position);
                break;
            default:
                destination = Vector3.zero;
                break;
        }
        agent.SetDestination(destination);
        //animator.ResetTrigger("DestinationReached");
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
        Character character = fsm.gameObject.GetComponent<Character>();
        character.ResetNeed(need);        
    }

    private Vector3 FindFood(Vector3 position)
    {
        return GameObject.FindGameObjectWithTag("Food").transform.position;
    }

    private Vector3 FindToilet(Vector3 position)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Toilet");
        return FindClosest(targets, position);
    }

    private Vector3 FindCoffee(Vector3 position)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Coffee");
        return FindClosest(targets, position);
    }


    private Vector3 FindClosest(GameObject[] targets, Vector3 position)
    {
        float minDist = Mathf.Infinity;
        int bestTarget = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            float newDist = Vector3.SqrMagnitude(position - targets[i].transform.position);
            if (minDist > newDist)
            {
                bestTarget = i;
                minDist = newDist;
            }
        }
        return targets[bestTarget].transform.position;
    }

}
