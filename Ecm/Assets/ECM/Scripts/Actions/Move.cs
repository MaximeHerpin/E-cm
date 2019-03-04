using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Move : EventAction
{
    GameObject destination;

    public Move(GameObject[] actors, GameObject destination) : base(actors)
    {
        this.destination = destination;
    }

    public override void OnEnter()
    {
        Vector3 destinationPosition = destination.transform.position;
        foreach (GameObject actor in actors)
        {
            NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError(string.Format("{0} was asked to move but has no NavMeshAgent Component", actor.name));
                return;
            }
            agent.SetDestination(destinationPosition);
        }
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        foreach (GameObject actor in actors)
        {
            NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
            agent.ResetPath();
        }
    }
}