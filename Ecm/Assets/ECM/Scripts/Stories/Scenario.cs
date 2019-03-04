using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Stories;

public class Scenario : MonoBehaviour {

    public Story[] stories;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void CheckStories()
    {
        foreach (Story story in stories)
        {
            story.UpdateStory();
        }
    }
}
