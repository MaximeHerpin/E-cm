using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : EventAction {

    public Explode(GameObject[] actors) : base(actors)
    {

    }
    public override void OnEnter() // vérifier que les acteurs existent
    {
        if (actors.Length == 0)
        {
            Debug.LogError(string.Format("{0} was asked to explod but does not exist", actor.name));
            return;
        }
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit()
    {
        for (i = 0; i < actors.Length; i++)
        {
            Destroy(actors[i]);
        }
    }
}
