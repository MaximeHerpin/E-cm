using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class FSMComponent : MonoBehaviour {

    public StateMachine FSM;
    public State state;
    public State[] states;
    public string trigger;
	// Use this for initialization
	void Start () {
        states = FSM.states.ToArray();
        state = FSM.states[0];
        state.action.OnEnter(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (trigger != "")
        {
            State destination = state.Transition(trigger, states);
            trigger = ""; // reset trigger
            if (destination != null)
            {
                state.action.OnLeave(this);
                state = destination;
                state.action.OnEnter(this);
                return; // execute OnUpdate next frame
            }
        }
        state.action.OnUpdate(this);
	}
}
