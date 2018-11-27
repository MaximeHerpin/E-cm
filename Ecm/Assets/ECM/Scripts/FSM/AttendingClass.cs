using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class AttendingClass : Action {

    AgendaComponent agenda;
    FSMComponent fsm;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(FSMComponent fsm)
    {
        TimeManager.instance.OnQuarterUpdate += CheckIfClassIsOver;
        agenda = fsm.gameObject.GetComponent<AgendaComponent>();
        this.fsm = fsm;
        //fsm.SetBool("GoToClass", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(FSMComponent fsm)
    {

    }

    public void CheckIfClassIsOver()
    {
        
        if(agenda.IsCurrentEventOver())
        {
            Debug.Log("executing");
            fsm.SetTrigger("ActivityFinished");
        }        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(FSMComponent fsm)
    {
        TimeManager.instance.OnQuarterUpdate -= CheckIfClassIsOver;
    }

}
