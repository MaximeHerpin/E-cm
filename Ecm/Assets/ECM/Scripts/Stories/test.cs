using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Stories;
using System.IO;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string path = Application.dataPath + "/unityTest.json";
        string dataAsJson = File.ReadAllText(path);
        JsonStory t = JsonUtility.FromJson<JsonStory>(dataAsJson);
        Debug.Log(t.Events[0].Actors[0]);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public struct MyObject
{
    [System.Serializable]
    public struct ArrayEntry
    {
        public int test;
    }

    public ArrayEntry[] Items;
}
