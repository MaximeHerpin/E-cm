using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    [System.Serializable]
    public class Story
    {
        public bool isOver = false;

        [SerializeField]
        public StoryEvent[] events = null;
        private int nextEventIndex;
        private StoryEvent currentEvent = null;

        public Story(int eventCount=0)
        {
            events = new StoryEvent[eventCount];
            nextEventIndex = 1;
            if (eventCount > 0)
                currentEvent = events[0];
        }

        public void UpdateStory()
        {
            if (currentEvent == null)
                currentEvent = events[0];
            switch (currentEvent.status)
            {
                case EventSatus.Happening:
                    currentEvent.Update();
                    break;
                case EventSatus.Over:
                    if (nextEventIndex < events.Length)
                    {
                        currentEvent = events[nextEventIndex];
                        nextEventIndex++;
                    }
                    else
                    {
                        isOver = true;
                    }
                    break;
                case EventSatus.WaitingToHappen:
                    currentEvent.CheckConditions();
                    break;                
            }
        }
    }
}