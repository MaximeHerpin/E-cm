using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agenda", menuName = "Custom/Agenda")]
public class Agenda : ScriptableObject {
    public AgendaEvent[] events;
    private int nextEventIndex = 0;

    public AgendaEvent NextEvent()
    {
        if (nextEventIndex < events.Length)
        {
            AgendaEvent curentEvent = events[nextEventIndex];
            nextEventIndex++;
            return curentEvent;
        }
        else
            return new AgendaEvent(new TimeOfDay(0, 0), new TimeOfDay(24, 59), "Doby is free !", Vector3.zero);
    }

    public void Initialize()
    {
        nextEventIndex = 0;
    }
}


[System.Serializable]
public class AgendaEvent
{
    public TimeOfDay startTime;
    public TimeOfDay endTime;
    public string description;
    public Vector3 position;

    public AgendaEvent(TimeOfDay startTime, TimeOfDay endTime, string description, Vector3 position)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.description = description;
        this.position = position;
    }

    public bool IsFinished(TimeOfDay time)
    {
        return time >= endTime;
    }

    public bool HasBegun(TimeOfDay time)
    {
        return time >= startTime;
    }
}
