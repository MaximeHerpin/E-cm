using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Conditions))]
public class ConditionsInspector : Editor
{
    Conditions cond;

    private void OnEnable()
    {
        cond = (Conditions)target;
    }

    private void Awake()
    {

    }


    public override void OnInspectorGUI()
    {
        if (cond.conditions != null)
        {
            foreach (KeyValuePair<string, bool> entry in cond.conditions)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entry.Key);
                EditorGUILayout.LabelField(entry.Value.ToString());
                EditorGUILayout.EndHorizontal();
            }
        }
    }

}
