using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class FSMInspector : Editor
{
    private void OnEnable()
    {
        StateMachine machine = (StateMachine)target;
        //ag.events = new Queue<AgendaEvent>(ag.eventList);
    }

    private void Awake()
    {

    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }


    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject as StateMachine != null)
        {
            FSMNodeEditor.ShowEditor();
            return true; //catch open file
        }

        return false; // let unity open the file
    }
}

