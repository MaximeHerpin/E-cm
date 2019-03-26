using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Agenda))]
public class AgendaInspector : Editor
{
    private void OnEnable()
    {
        Agenda ag = (Agenda)target;
        //ag.events = new Queue<AgendaEvent>(ag.eventList);
    }

    private void Awake()
    {
        
    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.fieldWidth = 1;
        EditorGUIUtility.labelWidth = 35;
        Agenda ag = (Agenda)target;
        if (ag.events == null && ag.eventList == null)
            return;

        foreach(AgendaEvent ev in ag.eventList)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            int h1 = ev.startTime.hours;
            int h2 = ev.endTime.hours;
            int m1 = ev.startTime.minutes;
            int m2 = ev.endTime.minutes;
            EditorGUILayout.LabelField(string.Format("From {0:00}:{1:00} to {2:00}:{3:00}", h1, m1, h2, m2), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 35;
            EditorGUILayout.IntField("From ", ev.startTime.hours);
            EditorGUIUtility.labelWidth = 5;
            EditorGUILayout.IntField(":",ev.startTime.minutes);
            EditorGUIUtility.labelWidth = 20;
            EditorGUILayout.IntField(" to ", ev.endTime.hours);
            EditorGUIUtility.labelWidth = 5;
            EditorGUILayout.IntField(":", ev.endTime.minutes);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Description : " + ev.description);
            EditorGUILayout.LabelField("Location : " + ev.location.Replace('_', ' '));
            EditorGUILayout.EndVertical();
        }
    }

}
