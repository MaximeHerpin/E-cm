using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class FSMComponent : MonoBehaviour {

    public StateMachine FSM;
    public State state;
    public State[] states;
    public string trigger;
    public object passedData;

	// Use this for initialization
	void Start () {
        states = FSM.states.ToArray();
        foreach (State s in states)
        {
            Debug.Log(s.name);
            var type = System.Type.GetType(s.actionType);
            s.action = (Action)System.Activator.CreateInstance(type);
        }
        state = FSM.states[0];
        state.action.OnStateEnter(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (trigger != "")
        {
            State destination = state.Transition(trigger, states);
            trigger = ""; // reset trigger
            if (destination != null)
            {
                Debug.Log(destination.name);
                state.action.OnStateExit(this);
                state = destination;
                state.action.OnStateEnter(this);
                return; // execute OnUpdate next frame
            }
        }
        state.action.OnStateUpdate(this);
	}
       

    public void SetTrigger(string trigger)
    {
        this.trigger = trigger;
    }

    public void SetData(object data)
    {
        passedData = data;
    }

    public object GetData()
    {
        return passedData;
    }


}
