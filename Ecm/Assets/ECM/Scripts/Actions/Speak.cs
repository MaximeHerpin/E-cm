using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speak : EventAction {

    public Move(GameObject[] actors, Dialogue dialogue) : base(actors)
    {
        this.dialogue = dialogue;
    }
    public override void OnEnter() // vérifier que les acteurs existent
    {
        if (actors.Length == 0)
        {
            Debug.LogError(string.Format("{0} was asked to speak but does not exist", actor.name));
            return;
        }
    }
    public override void OnUpdate()
    {

    }
}


public class Dialogue
{

}