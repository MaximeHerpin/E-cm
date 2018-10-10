using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public Camera cam;
    public Transform[] positions;
    private UnityEngine.AI.NavMeshAgent agent;
    private int positionIndex = 0;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    positionIndex = (positionIndex + 1) % positions.Length;
                    agent.SetDestination(positions[positionIndex].position);
                }
            }
        }
    }
}