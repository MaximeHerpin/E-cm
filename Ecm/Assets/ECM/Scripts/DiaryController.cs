using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiaryController : MonoBehaviour {

    public GameObject content;
    public GameObject listItemPrefab;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;

    public void AddEntries(Queue<DiaryEntry> entries)
    {
        foreach (DiaryEntry entry in entries)
            AddEntry(entry);
    }

    public void UpdateNameAndStatus(string name, string status)
    {
        this.nameText.text = name;
        this.statusText.text = status;
    }

	void Update () {
		
	}

    public void AddEntry(DiaryEntry entry)
    {
        GameObject item = Instantiate(listItemPrefab, content.transform) as GameObject;
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.time;
        item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.description;
    }

}

public struct DiaryEntry
{
    public string time;
    public string description;

    public DiaryEntry(TimeOfDay time, string description)
    {
        this.time = time.ToString();
        this.description = description;
    }
}
