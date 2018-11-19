using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAgendas : ScriptableWizard
{
    public int number = 5;
    public bool deleteCurrentAgendas = true;

    [MenuItem("Tools/Create Agendas")]
    static void CreateAgendasWizard()
    {
        ScriptableWizard.DisplayWizard<CreateAgendas>("Create Agendas", "create");
    }

    private void OnWizardCreate()
    {
        string path = "Assets/ECM/Resources/Agendas";
        string parent = "Assets/ECM/Resources";
        if (deleteCurrentAgendas)
        {
            FileUtil.DeleteFileOrDirectory(path);
            AssetDatabase.CreateFolder(parent, "Agendas");
        }
        AssetDatabase.Refresh();
        for (int i=0; i<number; i++)
        {
            Agenda ag = ScriptableObject.CreateInstance<Agenda>();
            AssetDatabase.CreateAsset(ag, path + "/agenda" + i.ToString() + ".asset");
        }
        AssetDatabase.SaveAssets();
    }
}
