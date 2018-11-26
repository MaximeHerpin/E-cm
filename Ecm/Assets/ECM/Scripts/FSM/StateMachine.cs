using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM // Finished State Machine
{
    [CreateAssetMenu(fileName = "FSM", menuName = "Custom/FSM")]
    public class StateMachine : ScriptableObject {

        public List<State> states;

        public StateMachine()
        {
            states = new List<State>();
        }

        public void AddState(Vector2 pos)
        {
            states.Add(new State(pos));
        }
    }

    [System.Serializable]
    public class State
    {
        public string name;
        public List<Transition> transitions;
        public Action action;
        public Rect rect;

        public State(Vector2 position)
        {
            transitions = new List<Transition>();
            rect = new Rect(position, new Vector2(100, 100));
            name = "coucou";
        }

        public State Transition(string trigger, State[] states)
        {
            foreach(Transition trans in transitions)
            {
                if(trans.trigger == trigger)
                {
                    return states[trans.destination];
                }
            }
            return null;
        }

        public void AddTransition(int other)
        {
            transitions.Add(new Transition(other, "defaultTrigger"));
        }
    }


    public abstract class Action
    {
        public abstract void OnEnter(FSMComponent fsm);

        public abstract void OnUpdate(FSMComponent fsm);

        public abstract void OnLeave(FSMComponent fsm);
    }

    [System.Serializable]
    public struct Transition
    {
        public int destination;
        public string trigger;

        public Transition(int destination, string trigger)
        {
            this.destination = destination;
            this.trigger = trigger;
        }
    }

}
