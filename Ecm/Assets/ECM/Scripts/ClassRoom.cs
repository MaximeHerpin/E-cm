using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ClassRoom: MonoBehaviour {

    public float density = .8f;
    public Vector3[] positions;
    public bool randomPositions = false;
    private int currentPositionIndex;


	void Start ()
    {
        GeneratePositions();
	}
	
    public Vector3 GetNextPosition()
    {
        Vector3 result = positions[currentPositionIndex];
        currentPositionIndex = (currentPositionIndex + 1) % positions.Length;
        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void GeneratePositions()
    {
        if (randomPositions)
        {
            GeneratePositionsRandom();
        }
        else
        {
            GeneratePositionsOnGrid();
        }
    }

    private void GeneratePositionsOnGrid()
    {
        Bounds bounds = GetComponent<BoxCollider>().bounds;
        int xNumber = (int)(bounds.size.x * density);
        int zNumber = (int)(bounds.size.z * density);

        if (xNumber * zNumber == 0)
        {
            xNumber = Mathf.Max(1, xNumber);
            zNumber = Mathf.Max(1, zNumber);
        }

        positions = new Vector3[xNumber * zNumber];        

        Vector3 startPos = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

        startPos += new Vector3(bounds.size.x - (xNumber - 1) / density, 0, bounds.size.z - (zNumber - 1) / density) / 2; // offset the start pos by half of the error on x-z Numbers
        
        for (int i = 0; i < xNumber; i++)
        {
            for (int j = 0; j < zNumber; j++)
            {
                positions[i*zNumber + j] = startPos + new Vector3(i / density, 0, j / density);
            }
        }

        ShufflePositions();
    }
    
    private void GeneratePositionsRandom()
    {
        // The following algorithme generates Poisson Disk Sampling (blue noise)
        Bounds bounds = GetComponent<BoxCollider>().bounds;
        float radius = 1 / density;
        float radiusSqr = Mathf.Pow(radius, 2);

        List<Vector3> points = new List<Vector3>();
        Queue<Vector3> emitingPoints = new Queue<Vector3>();

        Vector3 startPos = bounds.center + new Vector3(0, -bounds.extents.y, 0);
        points.Add(startPos);
        emitingPoints.Enqueue(startPos);

        int overflow = 0;

        while (emitingPoints.Count > 0 && overflow < 150)
        {
            Vector3 pos = emitingPoints.Dequeue();
            for (int i=0; i<15; i++)
            {
                Vector3 randomPoint = Random.insideUnitCircle.normalized * radius * (1 + Random.value); // Random vector with magnitude between radius and 2*radius
                randomPoint.z = randomPoint.y;
                randomPoint.y = 0;
                randomPoint += pos;

                if (!bounds.Contains(randomPoint)) // stop iteration if point is out of bounds
                    continue;

                bool discardPoint = false;
                foreach(Vector3 existingPoint in points) // Checking if new point is not too close from existing ones
                {
                    if (Vector3.SqrMagnitude(existingPoint - randomPoint) < radiusSqr)
                    {
                        discardPoint = true;
                        break;
                    }
                }
                if (discardPoint) // stop iteration if point is too close from another one
                    continue;

                emitingPoints.Enqueue(randomPoint);
                points.Add(randomPoint);

                overflow++;
            }
        }

        positions = points.ToArray();

    }

    private void OnDrawGizmosSelected()
    {
        if (positions == null)
            GeneratePositions();

        foreach(Vector3 pos in positions)
        {
            Gizmos.DrawSphere(pos, .1f);
        }
    }
    
    private void ShufflePositions()
    {
        int n = positions.Length;
        for(int i=1; i<n; i++)
        {
            int index = Random.Range(0, n);
            Vector3 tmp = positions[i];
            positions[i] = positions[index];
            positions[index] = tmp;
        }
    }

    private void OnValidate()
    {
        GeneratePositions();
    }

}
