using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class DieAction : EventAction
    {
        public DieAction(GameObject[] actors) : base(actors)
        {

        }
        public override void OnEnter() // vérifier que les acteurs existent
        {
            
        }
        public override void OnUpdate()
        {
            status = ActionStatus.Exit;
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