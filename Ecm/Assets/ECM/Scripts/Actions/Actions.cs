using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EventAction {

    protected GameObject[] actors;

    public EventAction(GameObject[] actors = null)
    {
        this.actors = actors;
    }

    virtual public void OnEnter() { }

    virtual public void OnUpdate(){}

    virtual public void OnExit(){}
    
}


Interact

See

Hear
