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
        Conditions.instance.conditions["test"] = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

