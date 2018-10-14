using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMood : MonoBehaviour {

    private Character character;
    public GameObject moodUi;
    public Transform canvas;
    private Text moodText;

	// Use this for initialization
	void Start () {
        character = GetComponent<Character>();
        GameObject moodInstance = Instantiate(moodUi, canvas);
        moodText = moodInstance.GetComponentInChildren<Text>();
        moodText.text = character.mood.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
