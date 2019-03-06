using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stories
{
    public class ExplodeAction : EventAction {

        public ExplodeAction(GameObject[] actors) : base(actors) { }

        public override void OnEnter() // vérifier que les acteurs existent
        {
            Debug.Log("Exploding");
            foreach(GameObject actor in actors)
            {
                Explode explodeComponent = actor.GetComponent<Explode>();
                if (explodeComponent != null)
                    explodeComponent.Boom();
            }
            
        }
        public override void OnUpdate()
        {
            status = ActionStatus.Exit;
        }
        public override void OnExit()
        {
            Debug.Log("Has exploded");
            for (int i=0; i<actors.Length; i++)
            {
                GameObject.Destroy(actors[i]);
            }
        }
    }
}
