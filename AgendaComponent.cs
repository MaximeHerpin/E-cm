using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgendaComponent : MonoBehaviour {
    public Agenda agenda;
    public AgendaEvent currentEvent;
    public bool auto_update = false; // set to false when the update is made trough the autonomous movement
    public bool FreeTime = false;

    void Start () {
        agenda.Initialize();
        currentEvent = agenda.NextEvent();
        FreeTime = !currentEvent.HasBegun(TimeManager.instance.timeOfDay);
        if (auto_update)
            TimeManager.instance.OnQuarterUpdate += CheckAgenda;
	}

    public void CheckAgenda()
    {
        TimeOfDay timeOfDay = TimeManager.instance.timeOfDay;
        if (currentEvent.IsFinished(timeOfDay))
        {
            currentEvent = agenda.NextEvent();            
        }
        FreeTime = !currentEvent.HasBegun(timeOfDay);

    }

	// Update is called once per frame
	void Update () {
		
	}
}
