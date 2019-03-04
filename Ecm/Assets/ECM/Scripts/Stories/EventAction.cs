using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stories
{
    [System.Serializable]
    public abstract class EventAction {

        protected GameObject[] actors;
        public ActionStatus status;

        public EventAction(GameObject[] actors = null)
        {
            this.actors = actors;
            status = ActionStatus.Start;
        }

        virtual public void OnEnter() {}

        virtual public void OnUpdate(){}

        virtual public void OnExit(){}
    
    }

    public enum ActionStatus
    {
        Start,
        Update,
        Exit
    }
}
