using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agenda", menuName = "Custom/Agenda")]
public class Agenda : ScriptableObject {
    public AgendaEvent[] eventList; // Must keep an array of event since unity doesn't serialize queues
    public Queue<AgendaEvent> events;

    public void Initialize()
    {
        if(events == null)
        {
            events = new Queue<AgendaEvent>(eventList);
        }
        foreach (AgendaEvent ev in events)
        {
            ev.classroom = GameObject.Find(ev.location);
        }
    }

    public void RandomDay()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("room");
        List<AgendaEvent> dayEvents = new List<AgendaEvent>();
        bool earlyMorning = Random.value < .5f;
        bool earlyAfternoon = Random.value < .5f;

        TimeOfDay time = earlyMorning ? new TimeOfDay(8, 0) : new TimeOfDay(8, 30); // time of first event
        for (int i=0; i<4; i++)
        {
            int duration = Random.value<0.5f ? 90 : 120; // random duration
            if (i == 0) // The first class duration is determined by it's start time
                duration = earlyMorning ? 120 : 90;
            if (i == 2) // The third class duration is determined by it's start time
                duration = earlyAfternoon ? 120 : 90;

            if (Random.value > .1) // slight chance of not having a class
            {
                GameObject location = rooms[Random.Range(0, rooms.Length - 1)]; // random classroom
                AgendaEvent newEvent = new AgendaEvent(time, time + duration, "yolo", location.name);
                dayEvents.Add(newEvent);
            }
                

            time = time + duration;
            if (i == 0 || i == 2) // breaks
                time = time + 15;

            if (i == 1) // lunch
                time = earlyAfternoon ? new TimeOfDay(13, 30) : new TimeOfDay(14, 0);
        }
        eventList = dayEvents.ToArray();
        events = new Queue<AgendaEvent>(dayEvents);
    }
    
}


[System.Serializable]
public class AgendaEvent
{
    public TimeOfDay startTime;
    public TimeOfDay endTime;
    public string description;
    public string location;
    public GameObject classroom;

    public AgendaEvent(TimeOfDay startTime, TimeOfDay endTime, string description, string location)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.description = description;
        this.location = location;
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
