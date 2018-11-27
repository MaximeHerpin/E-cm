using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FSM;

public class Socialize : Action {

    Character character;
    AgendaComponent agenda;
    DetectionComponent detection;
    NavMeshAgent nav;
    FSMComponent fsm;
    float detectionRadiusSqr;
    int friendsNearby;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(FSMComponent fsm)
    {
        TimeManager.instance.OnQuarterUpdate += CheckIfBreakIsOver;
        character = fsm.gameObject.GetComponent<Character>();
        detection = fsm.gameObject.GetComponent<DetectionComponent>();
        agenda = fsm.gameObject.GetComponent<AgendaComponent>();
        nav = fsm.gameObject.GetComponent<NavMeshAgent>();
        //nav.stoppingDistance = .5f;
        this.fsm = fsm;
        detectionRadiusSqr = Mathf.Pow(detection.detectionRadius,2);
        friendsNearby = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(FSMComponent fsm)
    {
        Vector3 pos = fsm.transform.position;
        int friends = 0;
        foreach (Character friend in character.friends)
        {
            if (Vector3.SqrMagnitude(fsm.transform.position - friend.transform.position) < detectionRadiusSqr)
            {
                pos += friend.transform.position;
                friends++;
            }
        }
        if (friends != friendsNearby) // only updating nav if 
        {
            nav.stoppingDistance = nav.radius*friends;
            friendsNearby = friends;
            nav.SetDestination(pos / (friends + 1));
        }

    }

    public void CheckIfBreakIsOver()
    {
        if (agenda.HasCurrentEventBegun())
        {
            fsm.SetTrigger("GoToClass");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(FSMComponent fsm)
    {
        TimeManager.instance.OnQuarterUpdate -= CheckIfBreakIsOver;
    }

}
