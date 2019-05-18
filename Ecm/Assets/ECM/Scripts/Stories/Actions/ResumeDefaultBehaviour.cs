using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stories
{
    public class DefaultBehaviourAction : EventAction
    {

        public DefaultBehaviourAction(GameObject[] actors) : base(actors)
        {
        }

        public override void OnEnter()
        {
            foreach (GameObject actor in actors)
            {
                Animator anim = actor.GetComponent<Animator>();
                if (anim == null)
                {
                    anim.SetBool("StoryEvent", false);
                }
            }
        }

        public override void OnUpdate()
        {
            status = ActionStatus.Exit;
        }

        public override void OnExit()
        {
        }
    }
}