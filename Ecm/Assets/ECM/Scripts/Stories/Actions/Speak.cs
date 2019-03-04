using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stories
{
    public class Speak : EventAction {

        private Dialogue dialogue; 
            
        public Speak(GameObject[] actors, Dialogue dialogue) : base(actors)
        {
            this.dialogue = dialogue;
        }
        public override void OnEnter() // vérifier que les acteurs existent
        {
            Debug.Log(dialogue);
        }
        public override void OnUpdate()
        {

        }
    }

    public class Dialogue
    {

    }
}