using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgendaManager : MonoBehaviour {

    public static AgendaManager instance;
    public Agenda[] agendas;

    private void Awake()
    {
        instance = this;
        agendas = Resources.LoadAll<Agenda>("Agendas");
    }

    void Start () {
        TimeManager.instance.OnQuarterUpdate += CheckAgendas;
        foreach (Agenda ag in agendas)
        {
            ag.Initialize();
        }
	}
	
	void Update () {
		
	}

    public void CheckAgendas()
    {
        foreach (Agenda ag in agendas)
        {
            TimeOfDay timeOfDay = TimeManager.instance.timeOfDay;
            if (ag.events.Peek().IsFinished(timeOfDay))
            {
                ag.events.Dequeue();
            }
        }
    }
}
