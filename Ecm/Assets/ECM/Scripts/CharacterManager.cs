using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    public static CharacterManager instance;
    public float needsBuildupSpeed = 1f;
    public float friendshipThreshold = .5f;
    public GameObject characterPrefab;
    public SpawnPoint[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        SpawnCharacters();
        ComputeFriendships();
	}
    

    public void SpawnCharacters()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            point.UpdatePoints();
            foreach (Vector3 pos in point.points)
            {
                Character character = Instantiate(characterPrefab, pos, Quaternion.identity).GetComponent<Character>();
                character.RandomizeCharacter();
            }
        }
    
    }
    
    public void ComputeFriendships()
    {
        // This function loops through every pair of characters and compute their likeness. past a likeness threshold, two characters are firends
        // likeness is evaluated with the cosine similarity
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playersCount = players.Length;
        for (int i=0; i<playersCount; i++)
        {
            Character c1 = players[i].GetComponent<Character>();
            Vector3 v1 = new Vector3(c1.physicalCondition, c1.studious, c1.social);
            v1 = (v1 - Vector3.one / 2).normalized; // center and normalize v1
            for (int j=i+1; j<playersCount; j++)
            {
                Character c2 = players[j].GetComponent<Character>();
                Vector3 v2 = new Vector3(c2.physicalCondition, c2.studious, c2.social);
                v2 = (v2 - Vector3.one / 2).normalized; // center and normalize v2

                float similarity = Vector3.Dot(v1, v2); // cosine similarity
                if (similarity > friendshipThreshold)
                {
                    c1.friends.Add(c2);
                    c2.friends.Add(c1);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            point.Draw();
        }
        //spawnPoints[1].Draw();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint.position, 1);
        }
    }
}

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public int number;
    public Vector3[] points;

    public void UpdatePoints()
    {
        points = new Vector3[number];
        float startOffset = Mathf.Sqrt(number) / 2;
        Vector3 startPos = position - new Vector3(startOffset, 0, startOffset);
        int squareRoot = (int)Mathf.Sqrt(number) + 1;
        for (int i = 0; i < squareRoot; i++)
        {
            for (int j = 0; j < squareRoot; j++)
            {
                if (i * squareRoot + j >= number) // Making sure not to create more than asked
                    return;
                Vector3 pos = startPos + (Vector3.forward * i + Vector3.right * j);
                points[i * squareRoot + j] = pos;
            }
        }
    }

    public void Draw()
    {
        UpdatePoints();
        foreach(Vector3 point in points)
        {
            Gizmos.DrawSphere(point, .1f);
        }
    }

}
