using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBreak : StateMachineBehaviour {

    Character character;
    AgendaComponent agenda;
    Animator anim;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.gameObject.GetComponent<Character>();
        agenda = animator.GetComponent<AgendaComponent>();
        anim = animator;

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

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
