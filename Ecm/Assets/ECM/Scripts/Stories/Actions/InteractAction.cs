using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stories
{
    public class InteractAction : EventAction
    {
        private GameObject target;
        private NavMeshAgent agent;

        public InteractAction(GameObject[] actors, GameObject target) : base(actors)
        {
            this.target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 destinationPosition = target.transform.position;
            foreach (GameObject actor in actors)
            {
                agent = actor.GetComponent<NavMeshAgent>();
                if (agent == null)
                {
                    Debug.LogError(string.Format("{0} was asked to move but has no NavMeshAgent Component", actor.name));
                    return;
                }
                //agent.ResetPath();
                agent.SetDestination(destinationPosition);                
            }
        }

        public override void OnUpdate()
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        status = ActionStatus.Exit;
                    }
                }
            }
        }

        public override void OnExit()
        {
            Debug.Log("Stop");
            foreach (GameObject actor in actors)
            {
                NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
                agent.ResetPath();
                Character character = actor.GetComponent<Character>();
                if (character != null)
                    character.AddDiaryEntry(string.Format("Interacted with {0}", target.name));
            }
        }
    }
}