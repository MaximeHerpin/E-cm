using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour {

    public static VisibilityManager instance = null;

    public int level = 4; // curent level seen by the camera
    private Dictionary<int, List<GameObject>> levelsDict; // Dictionary storring all GameObjects of each level

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }

    void Start () {
        levelsDict = new Dictionary<int, List<GameObject>>(); // initializing dictionary
        GameObject[] buidings = GameObject.FindGameObjectsWithTag("Building"); // Get all Building Objects
        foreach (GameObject building in buidings)
        {
            foreach (Transform child in building.transform) // Each Building Object has levels
            {
                string name = child.name;
                int childLevel = (int) char.GetNumericValue(name[name.Length - 1]); // Extract level from child name
                if (name[name.Length - 2] == '-')
                    childLevel *= -1;
                if (! levelsDict.ContainsKey(childLevel))
                    levelsDict[childLevel] = new List<GameObject>(); // Initialize List if level is seen for the first time
                levelsDict[childLevel].Add(child.gameObject);
            }
        }
	}
	
	void Update () {
        int direction = 0;
        if (Input.GetButtonDown("LevelUp"))
            direction += 1;
        if (Input.GetButtonDown("LevelDown"))
            direction -= 1;

        if (direction != 0)
        {
            ChangeLevel(direction);
        }
	}



    private void ChangeLevel(int direction)
    {
        if (direction == 1 && levelsDict.ContainsKey(level + 1)) // revealing upper level
        {
            foreach (GameObject ob in levelsDict[level + 1])
            {
                ob.SetActive(true);
            }
        }
        else if (direction == -1 && levelsDict.ContainsKey(level)) // hidding current level
        {
            foreach(GameObject ob in levelsDict[level])
            {
                ob.SetActive(false);
            }
        }
        level += direction;
    }
}
