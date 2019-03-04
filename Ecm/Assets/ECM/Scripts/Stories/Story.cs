using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class Story
    {
        public bool isOver = false;

        private StoryEvent[] events = null;
        private int nextEventIndex = 0;
        private StoryEvent currentEvent = null;

        public void UpdateStory()
        {
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