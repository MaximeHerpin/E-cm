using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    [System.Serializable]
    public class StoryEvent
    {
        public EventSatus status;
        [SerializeField]
        private TimeOfDay timeMin;
        [SerializeField]
        private TimeOfDay timeMax;
        private BooleanExpression conditions;
        private string[] consequences;
        [SerializeField]
        private EventAction action;

        public StoryEvent(TimeOfDay timeMin, TimeOfDay timeMax, EventAction action, string conditions, string[] consequences)
        {
            this.timeMin = timeMin;
            this.timeMax = timeMax;
            this.action = action;
            this.conditions = new BooleanExpression(conditions);
            this.consequences = consequences;
            status = EventSatus.WaitingToHappen;
        }

        public void Update()
        {
            switch (action.status)
            {
                case ActionStatus.Start:
                    action.OnEnter();
                    action.status = ActionStatus.Update;
                    break;
                case ActionStatus.Update:
                    action.OnUpdate();
                    break;
                case ActionStatus.Exit:
                    action.OnExit();
                    foreach (string condition in consequences)
                        Conditions.instance.conditions[condition] = true;
                    status = EventSatus.Over;
                    break;
            }
        }

        public void CheckConditions()
        {
            TimeOfDay timeOfDay = TimeManager.instance.timeOfDay;
            //Debug.Log(timeMin <= timeOfDay);
            if (timeMin <= timeOfDay && timeOfDay <= timeMax)
            {
                Debug.Log("Time");
                if (conditions.Eval())
                    status = EventSatus.Happening; // all conditions are fullfilled, change status
            }
        }
    }

    public enum EventSatus
    {
        Over,
        Happening,
        WaitingToHappen
    }
}
