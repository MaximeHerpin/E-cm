using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgendaComponent : MonoBehaviour {
    public Agenda agenda;
    public AgendaEvent currentEvent;
    public bool auto_update = false; // set to false when the update is made trough the autonomous movement
    public bool FreeTime = false;


    void Start () {
        if (agenda == null)
        {
            Agenda[] agendas = Resources.LoadAll<Agenda>("Agendas");
            agenda = agendas[Random.Range(0, agendas.Length)];
        }

        currentEvent = agenda.events.Peek();
        FreeTime = !currentEvent.HasBegun(TimeManager.instance.timeOfDay);
        if (auto_update)
            TimeManager.instance.OnQuarterUpdate += CheckAgenda;
	}

    public void CheckAgenda()
    {
        FreeTime = !agenda.events.Peek().HasBegun(TimeManager.instance.timeOfDay);
        currentEvent = agenda.events.Peek();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
