using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;


[System.Serializable]
public class Break : Action
{
    Character character;
    AgendaComponent agenda;
    FSMComponent fsm;
    NavMeshAgent nav;
    bool eat = false;

    public override void OnStateEnter(FSMComponent fsm)
    {
        character = fsm.gameObject.GetComponent<Character>();
        agenda = fsm.GetComponent<AgendaComponent>();
        this.fsm = fsm;
        nav = fsm.gameObject.GetComponent<NavMeshAgent>();

        TimeManager.instance.OnQuarterUpdate += CheckIfBreakIsOver;
        CheckIfBreakIsOver();
        if (eat)
        {
            eat = false;
            return;
        }


        if (character.NeedsToilet())
        {
            fsm.SetTrigger("Toilet");
            fsm.SetData((int)1);
            return;
        }

        if (TimeManager.instance.timeOfDay >= new TimeOfDay(11, 30) && TimeManager.instance.timeOfDay <= new TimeOfDay(14, 0) && character.NeedsFood())
        {
            fsm.SetTrigger("Eat");
            fsm.SetData((int)0);
            return;
        }


        if (character.NeedsCafein())
        {
            fsm.SetTrigger("Coffee");
            fsm.SetData((int)2);
            return;
        }

        Vector3 socialPlace = FindClosest(GameObject.FindGameObjectsWithTag("SocialPlace"), fsm.transform.position).GetComponent<ClassRoom>().GetNextPosition();
        nav.SetDestination(socialPlace);
        nav.stoppingDistance = 8;
        if (character.social > .5f)
            fsm.SetTrigger("Socialize");
    }

    override public void OnStateUpdate(FSMComponent fsm)
    {
        if (TimeManager.instance.timeOfDay >= new TimeOfDay(11, 30) && TimeManager.instance.timeOfDay <= new TimeOfDay(14, 0) && character.NeedsFood())
        {
            fsm.SetTrigger("Eat");
            fsm.SetData((int)0);
            return;
        }

        if (character.NeedsToilet())
        {
            fsm.SetTrigger("Toilet");
            fsm.SetData((int)1);
            return;
        }

        if (character.NeedsCafein())
        {
            fsm.SetTrigger("Coffee");
            fsm.SetData((int)2);
            return;
        }
    }

    public void CheckIfBreakIsOver()
    {
        if (agenda.HasCurrentEventBegun())
        {
            fsm.SetTrigger("GoToClass");
            eat = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(FSMComponent fsm)
    {
        TimeManager.instance.OnQuarterUpdate -= CheckIfBreakIsOver;
    }

    private GameObject FindClosest(GameObject[] targets, Vector3 position)
    {
        float minDist = Mathf.Infinity;
        int bestTarget = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            float newDist = Vector3.SqrMagnitude(position - targets[i].transform.position);
            if (minDist > newDist)
            {
                bestTarget = i;
                minDist = newDist;
            }
        }
        return targets[bestTarget];
    }
}
