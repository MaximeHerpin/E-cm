using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgendaComponent : MonoBehaviour {
    public Agenda agenda;
    public AgendaEvent currentEvent;


    void Start () {
        if (agenda == null)
        {
            Agenda[] agendas = Resources.LoadAll<Agenda>("Agendas");
            agenda = agendas[Random.Range(0, agendas.Length)];
        }

        currentEvent = agenda.events.Peek();
	}

    public bool IsCurrentEventOver()
    {
        if (agenda.events.Count == 0)
            return true;
        if(currentEvent != agenda.events.Peek())
        {
            currentEvent = agenda.events.Peek();
            return true;
        }
        return false;
    }

    public bool HasCurrentEventBegun()
    {
        if (agenda.events.Count == 0)
            return false;
        return agenda.events.Peek().HasBegun(TimeManager.instance.timeOfDay);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
