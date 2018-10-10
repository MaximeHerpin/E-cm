using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character: MonoBehaviour{

    public string realName;
    public float physicalCondition = 1f; // Will influence speed, activities, ect
    public Mood mood;

    public float studious = 1; // How serious in studies
    public float social = 1; // Qualifies interactions

    private void Start()
    {
        gameObject.name = realName;
    }
}


public enum Mood {Calm, Happy, Flirty, Tired, Bored, Depressed, Sad, Angry, Hungry, Thirsty, Neutral}