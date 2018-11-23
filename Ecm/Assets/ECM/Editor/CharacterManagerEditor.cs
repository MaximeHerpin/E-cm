using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterManager))]
public class PositionHandleExampleEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        CharacterManager manager = (CharacterManager)target;

        foreach (SpawnPoint spawnPoint in manager.spawnPoints)
        {
            spawnPoint.position = Handles.PositionHandle(spawnPoint.position, Quaternion.identity);
        }
    }
}
