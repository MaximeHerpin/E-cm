using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class Die : EventAction
    {
        public Die(GameObject[] actors) : base(actors)
        {

        }
        public override void OnEnter() // vérifier que les acteurs existent
        {
            
        }
        public override void OnUpdate()
        {

        }
        public override void OnExit()
        {
            for (int i = 0; i < actors.Length; i++)
            {
                GameObject.Destroy(actors[i]);
            }
        }
    }
}