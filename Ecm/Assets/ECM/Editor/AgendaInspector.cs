using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Agenda))]
public class AgendaInspector : Editor
{

    public override void OnInspectorGUI()
    {
        Agenda ag = (Agenda)target;
        foreach(AgendaEvent ev in ag.events)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            int h1 = ev.startTime.hours;
            int h2 = ev.endTime.hours;
            int m1 = ev.startTime.minutes;
            int m2 = ev.endTime.minutes;
            EditorGUILayout.LabelField(string.Format("From {0:00}:{1:00} to {2:00}:{3:00}", h1, m1, h2, m2), EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Description : " + ev.description);
            EditorGUILayout.LabelField("Location : " + ev.location.Replace('_', ' '));
            EditorGUILayout.EndVertical();
        }
    }

}
