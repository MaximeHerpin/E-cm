﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public static TimeManager instance;
    public TimeOfDay timeOfDay;
    public delegate void Action();
    public event Action OnQuarterUpdate; // Will call event every 15 minutes (in game time)

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        timeOfDay = new TimeOfDay(7, 59);
        InvokeRepeating("UpdateTime", 5, 20); // UpdateTime will be called every 5 seconds (in game time)
    }
	

    public void UpdateTime()
    {
        timeOfDay.AddOneMinute();
        DisplayTime();
        if (timeOfDay.IsQuarterHour()) // Will be true every 15 in game minutes
        {
            if(OnQuarterUpdate != null)
                OnQuarterUpdate();
        }
    }

    public void DisplayTime()
    {
        timeOfDay.Display();
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Time.timeScale *= 2;
        }
        if (Input.GetKeyDown("m"))
        {
            Time.timeScale /= 2;
        }
    }
}

[System.Serializable]
public class TimeOfDay
{
    public int hours;
    public int minutes; // Those  are public so they can be set in Editor
    public static TimeOfDay FirstHour = new TimeOfDay(8, 0);
    public static TimeOfDay LastHour = new TimeOfDay(18, 0);

    public TimeOfDay(int hours, int minutes)
    {
        this.minutes = minutes;
        this.hours = hours;
    }
    
    public void AddOneMinute()
    {
        minutes += 1;
        if(minutes >= 60)
        {
            minutes = 0;
            hours += 1;
            if (hours >= 24)
                hours = 0;
        }
    }

    public void Display()
    {
        string text = "";
        if (hours < 10)
            text += "0";
        text += hours.ToString() + ":";
        if (minutes < 10)
            text += "0";
        text += minutes.ToString();        
        Debug.Log(text);
    }

    public bool IsQuarterHour()
    {
        return minutes % 15 == 0;
    }

    public static bool operator >=(TimeOfDay t1, TimeOfDay t2)
    {
        if (t1.hours < t2.hours)
        {
            return false;
        }
        else if (t1.hours == t2.hours && t1.minutes < t2.minutes)
        {
            return false;
        }
        else
            return true;
    }

    public static bool operator <=(TimeOfDay t1, TimeOfDay t2)
    {
        return t2 >= t1;
    }

    public static TimeOfDay operator +(TimeOfDay t1, int minutes)
    {
        int resultMinutes = (t1.minutes + minutes % 60) % 60;
        int resultHours = (t1.hours + minutes/60 + (t1.minutes + minutes % 60) / 60) % 24;
        return new TimeOfDay(resultHours, resultMinutes);
    }

    public TimeOfDay Copy()
    {
        return new TimeOfDay(hours, minutes);
    }

}
