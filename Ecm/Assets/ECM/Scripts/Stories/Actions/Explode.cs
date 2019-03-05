using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stories
{
    public class Explode : EventAction {
        public int boomCount = 0;

        public Explode(GameObject[] actors) : base(actors) { }

        public override void OnEnter() // vérifier que les acteurs existent
        {
            Debug.Log("Exploding");
            boomCount = 0;
        }
        public override void OnUpdate()
        {
            Debug.Log("BOOM");
            if (boomCount > 5)
            {
                status = ActionStatus.Exit;
            }
            boomCount++;
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
