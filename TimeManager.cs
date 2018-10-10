using System.Collections;
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
        Time.timeScale = 1f;
        timeOfDay = new TimeOfDay(7, 59);
        InvokeRepeating("UpdateTime", 5, 5); // UpdateTime will be called every 5 seconds (in game time)
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
}

[System.Serializable]
public struct TimeOfDay
{
    public int minutes; // Those  are public so they can be set in Editor
    public int hours;
    private float floatingTime;

    public TimeOfDay(int hours, int minutes)
    {
        this.minutes = minutes;
        this.hours = hours;
        floatingTime = 0f;
    }
    
    public void Add(float amount)
    {
        floatingTime += amount;
        if (floatingTime > 1000)
        {
            AddOneMinute();
            floatingTime = floatingTime - 1000;
        }
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

}
