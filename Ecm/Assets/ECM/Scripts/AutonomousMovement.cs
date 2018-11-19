using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent), typeof(AgendaComponent))]

public class AutonomousMovement : MonoBehaviour {

    private NavMeshAgent agent;
    private AgendaComponent ag;

    public Vector3 lastDestination = Vector3.zero;


    void Start () {
        ag = GetComponent<AgendaComponent>();
        agent = GetComponent<NavMeshAgent>();
        TimeManager.instance.OnQuarterUpdate += CheckAgendaForDestination;
        agent.speed *= (1 + GetComponent<Character>().physicalCondition)/2;
	}

    void CheckAgendaForDestination()
    {
        ag.CheckAgenda(); // Update agenda before using it
        Vector3 destination = ag.currentEvent.classroom.transform.position;
        if (destination != lastDestination && !ag.FreeTime) // If not in required position
        {
            if (!agent.pathPending) // Following conditions test if agent is moving, or tries to
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        agent.SetDestination(destination); // change agent destination
                        lastDestination = destination;
                    }
                }
            }
        }
    }
	
}
