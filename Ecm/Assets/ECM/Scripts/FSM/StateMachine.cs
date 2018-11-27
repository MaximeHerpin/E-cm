using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM // Finished State Machine
{
    [CreateAssetMenu(fileName = "FSM", menuName = "Custom/FSM")]
    public class StateMachine : ScriptableObject {

        public List<State> states;
        public int entryState = 0;

        public StateMachine()
        {
            states = new List<State>();
        }

        public void AddState(Vector2 pos)
        {
            states.Add(new State(pos));
        }

        public void DeleteState(int index)
        {
            states.RemoveAt(index);
            if(entryState > index)
            {
                entryState -= 1;
                if (entryState < 0)
                    entryState = 0;
            }
            else if (entryState == index)
            {
                entryState = 0;
            }

            foreach (State state in states)
            {
                for (int i=0; i<state.transitions.Count; i++)
                {
                    if (state.transitions[i].destination == index)
                    {
                        state.transitions.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }

    [System.Serializable]
    public class State
    {
        public string name;
        public List<Transition> transitions;
        public Action action;
        public string actionType;
        public Rect rect; // used for graph editor
        public int actionIndex = 0; // used for graph editor

        public State(Vector2 position)
        {
            transitions = new List<Transition>();
            rect = new Rect(position, new Vector2(150, 60));
            name = "new state";
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

    [System.Serializable]
    public abstract class Action
    {
        public abstract void OnStateEnter(FSMComponent fsm);

        public abstract void OnStateUpdate(FSMComponent fsm);

        public abstract void OnStateExit(FSMComponent fsm);
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
