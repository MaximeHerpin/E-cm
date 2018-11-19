using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionComponent : MonoBehaviour {

    public GameObject[] neighbours;
    public float detectionRadius = 5f;
    public int updatePeriod = 30; // Only execute computation intensive function every x frame
    private int frameCount;
    private LayerMask mask;
    private Collider[] neighboursCache; // Will store result of OverlapSphereNonAlloc function
    private int neighboursMaxNumber = 10;
    public int lastRealNeighbour = 0; // Index of last real neighbour;

    void Start () {
        neighbours = new GameObject[neighboursMaxNumber]; // Maximum of 10 neighbours
        neighboursCache = new Collider[neighboursMaxNumber];
        frameCount = Random.Range(0, updatePeriod - 1); // Random offset in frame count so that all character update in different frames
        mask = LayerMask.GetMask("Detectable");

    }
	
	// Update is called once per frame
	void Update () {
        frameCount++;
        if (frameCount >= updatePeriod)
        {
            frameCount = 0;
            DetectNeighbours();
        }
	}

    private void DetectNeighbours()
    {
        int collisionNumber = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, neighboursCache, mask);
        lastRealNeighbour = 0;
        for (int i=0; i< collisionNumber; i++)
        {
            Collider col = neighboursCache[i];
            if (col != null && ! col.gameObject.Equals(this.gameObject))
            {
                if (Vector3.Distance(col.transform.position, transform.position) < detectionRadius)
                {
                    neighbours[lastRealNeighbour] = col.gameObject;
                    lastRealNeighbour++;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
