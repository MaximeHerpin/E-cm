using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Stories
{
    public class MoveAction : EventAction
    {
        private GameObject destination;
        private NavMeshAgent agent;

        public MoveAction(GameObject[] actors, GameObject destination) : base(actors)
        {
            this.destination = destination;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 destinationPosition = destination.transform.position;
            foreach (GameObject actor in actors)
            {
                agent = actor.GetComponent<NavMeshAgent>();
                if (agent == null)
                {
                    Debug.LogError(string.Format("{0} was asked to move but has no NavMeshAgent Component", actor.name));
                    return;
                }
                agent.stoppingDistance = 1f;
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(destinationPosition, path);
                agent.SetPath(path);
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
            foreach (GameObject actor in actors)
            {
                NavMeshAgent agent = actor.GetComponent<NavMeshAgent>();
                agent.ResetPath();
                Character character = actor.GetComponent<Character>();
                if (character != null)
                    character.AddDiaryEntry(string.Format("Went to {0}", destination.name));
            }
        }
    }
}