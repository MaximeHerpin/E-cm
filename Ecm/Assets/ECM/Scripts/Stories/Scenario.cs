using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Stories;
using System.IO;

public class Scenario : MonoBehaviour {

    public string pathToJsonFolder = "ECM/Stories";
    public Story[] stories;
    

	// Use this for initialization
	void Start () {
        JsonConverter.pathToJsonFolder = Path.Combine(Application.dataPath, pathToJsonFolder);
        stories = JsonConverter.GetAllStories();
	}
	
	// Update is called once per frame
	void Update () {
        if (stories != null)
            CheckStories();
    }


    void CheckStories()
    {
        foreach (Story story in stories)
        {
            if (! story.isOver)
                story.UpdateStory();
        }
    }
}
