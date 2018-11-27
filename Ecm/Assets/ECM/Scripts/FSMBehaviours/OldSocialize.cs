using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OldSocialize : StateMachineBehaviour {

    Character character;
    AgendaComponent agenda;
    DetectionComponent detection;
    NavMeshAgent nav;
    Animator anim;
    float detectionRadiusSqr;
    int friendsNearby;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TimeManager.instance.OnQuarterUpdate += CheckIfBreakIsOver;
        character = animator.gameObject.GetComponent<Character>();
        detection = animator.gameObject.GetComponent<DetectionComponent>();
        agenda = animator.gameObject.GetComponent<AgendaComponent>();
        nav = animator.gameObject.GetComponent<NavMeshAgent>();
        //nav.stoppingDistance = .5f;
        anim = animator;
        detectionRadiusSqr = Mathf.Pow(detection.detectionRadius,2);
        friendsNearby = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 pos = animator.transform.position;
        int friends = 0;
        foreach (Character friend in character.friends)
        {
            if (Vector3.SqrMagnitude(animator.transform.position - friend.transform.position) < detectionRadiusSqr)
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
            anim.SetBool("GoToClass", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
