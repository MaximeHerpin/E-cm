using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour {

    public static SelectionManager instance;
    public Camera cam;
    public Character selectedCharacter = null;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (! EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Character character = hit.collider.gameObject.GetComponent<Character>();
                    if (character != null)
                    {
                        if (selectedCharacter != null)
                            selectedCharacter.Deselect();
                        character.Select();
                        selectedCharacter = character;
                    }
                }
            }
        }
    }
}
