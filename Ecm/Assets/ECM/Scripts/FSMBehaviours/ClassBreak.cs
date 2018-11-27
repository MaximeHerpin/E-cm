using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClassBreak : StateMachineBehaviour {

    Character character;
    AgendaComponent agenda;
    Animator anim;
    NavMeshAgent nav;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.gameObject.GetComponent<Character>();
        agenda = animator.GetComponent<AgendaComponent>();
        anim = animator;
        nav = animator.gameObject.GetComponent<NavMeshAgent>();

        TimeManager.instance.OnQuarterUpdate += CheckIfBreakIsOver;
        if (character.NeedsToilet())
        {
            animator.SetTrigger("Toilet");
            animator.SetInteger("NeedId", 1);
            return;
        }

        if (TimeManager.instance.timeOfDay >= new TimeOfDay(11,30) && TimeManager.instance.timeOfDay <= new TimeOfDay(14, 0) && character.NeedsFood())
        {
            animator.SetTrigger("Eat");
            animator.SetInteger("NeedId", 0);
            return;
        }


        if (character.NeedsCafein())
        {
            animator.SetTrigger("Coffee");
            animator.SetInteger("NeedId", 2);
            return;
        }

        Vector3 socialPlace = FindClosest(GameObject.FindGameObjectsWithTag("SocialPlace"), animator.transform.position).GetComponent<ClassRoom>().GetNextPosition();
        nav.SetDestination(socialPlace);
        //nav.stoppingDistance = 8;
        if (character.social > .5f)
            animator.SetTrigger("Socialize");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (TimeManager.instance.timeOfDay >= new TimeOfDay(11, 30) && TimeManager.instance.timeOfDay <= new TimeOfDay(14, 0) && character.NeedsFood())
        {
            animator.SetTrigger("Eat");
            animator.SetInteger("NeedId", 0);
            return;
        }

        if (character.NeedsToilet())
        {
            animator.SetTrigger("Toilet");
            animator.SetInteger("NeedId", 1);
            return;
        }

        if (character.NeedsCafein())
        {
            animator.SetTrigger("Coffee");
            animator.SetInteger("NeedId", 2);
            return;
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
        TimeManager.instance.OnQuarterUpdate -= CheckIfBreakIsOver;
    }

    private GameObject FindClosest(GameObject[] targets, Vector3 position)
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
        return targets[bestTarget];
    }
}
