using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendingClass : StateMachineBehaviour {

    AgendaComponent agenda;
    Animator anim;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TimeManager.instance.OnQuarterUpdate += CheckIfClassIsOver;
        agenda = animator.gameObject.GetComponent<AgendaComponent>();
        anim = animator;
        animator.SetBool("GoToClass", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public void CheckIfClassIsOver()
    {
        
        if(agenda.IsCurrentEventOver())
        {
            Debug.Log("executing");
            anim.SetTrigger("ActivityFinished");
        }        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TimeManager.instance.OnQuarterUpdate -= CheckIfClassIsOver;
    }

}
