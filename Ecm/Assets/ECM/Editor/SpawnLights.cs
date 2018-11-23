using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnLights : ScriptableWizard
{
    public float density = 0.3f;
    public float intensity = 3;
    public Color color = Color.white;

    [MenuItem("Tools/SpawnLights")]
    static void CreateLightsWizzard()
    {
        ScriptableWizard.DisplayWizard<SpawnLights>("Create Lights", "create");
    }


    private void OnWizardCreate()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("room");
        foreach (GameObject room in rooms)
        {
            Light[] oldLights = room.GetComponentsInChildren<Light>();
            foreach (Light light in oldLights)
            {
                GameObject.DestroyImmediate(light.gameObject); // Removing previous lights
            }

            Bounds bounds = room.GetComponent<BoxCollider>().bounds;
            int xNumber = (int)(bounds.size.x * density);
            int zNumber = (int)(bounds.size.z * density);

            if (xNumber * zNumber == 0)
                continue;

            Vector3 startPos = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            startPos += new Vector3(bounds.size.x / xNumber, 0, bounds.size.z / zNumber) /2; // offset the start pos by half of the error on x/zNumber

            for (int i=0; i<xNumber; i++)
            {
                for(int j=0; j<zNumber; j++)
                {
                    GameObject light = new GameObject("light");
                    light.transform.position = startPos + new Vector3(i / density, 0, j / density);
                    light.transform.parent = room.transform; // parenting new light to room
                    light.transform.Rotate(new Vector3(90, 0, 0));
                    Light lightInfo = light.AddComponent<Light>(); // Adding Light component
                    lightInfo.type = LightType.Area;
                    lightInfo.intensity = intensity;
                    lightInfo.color = color;
                    lightInfo.shadows = LightShadows.Soft;
                }
            }

        }
    }
}
