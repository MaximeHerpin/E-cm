using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenameClassRooms {

	[MenuItem("Tools/Rename rooms")]
    static void RenameClassRoomsWizzard()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("room");
        int firstRoom = 200;
        for (int i=0; i<rooms.Length; i++)
        {
            if(!rooms[i].name.Contains("amphi"))
                rooms[i].name = "salle_" + (firstRoom + i).ToString();
        }
    }
}
