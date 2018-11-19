using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour {

    public GameObject Character;
    public int number = 50;
    public float spacing = 1f;

	// Use this for initialization
	void Start () {
        float startOffset = Mathf.Sqrt(number) * spacing / 2;
        Vector3 startPos = transform.position - new Vector3(startOffset, 0, startOffset);
        int squareRoot = (int)Mathf.Sqrt(number) + 1;
        for (int i=0; i<squareRoot; i++)
        {
            for (int j=0; j<squareRoot; j++)
            {
                if (i * squareRoot + j >= number) // Making sure not to create more than asked
                    return;
                Vector3 pos = startPos + (Vector3.forward * i + Vector3.right * j) * spacing;
                Instantiate(Character, pos, Quaternion.identity);
            }
        }
	}
	
	
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float startOffset = Mathf.Sqrt(number) * spacing / 2;
        Vector3 startPos = transform.position - new Vector3(startOffset, 0, startOffset);
        int squareRoot = (int)Mathf.Sqrt(number) + 1;
        for (int i = 0; i < squareRoot; i++)
        {
            for (int j = 0; j < squareRoot; j++)
            {
                if (i * squareRoot + j >= number) // Making sure not to create more than asked
                    return;
                Vector3 pos = startPos + (Vector3.forward * i + Vector3.right * j) * spacing;
                Gizmos.DrawSphere(pos, .1f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
