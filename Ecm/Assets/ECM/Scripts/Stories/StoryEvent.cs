using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class StoryEvent
    {
        public EventSatus status;

        public void Update()
        {

        }

        public void CheckConditions()
        {

        }
    }

    public enum EventSatus
    {
        Over,
        Happening,
        WaitingToHappen
    }
}
