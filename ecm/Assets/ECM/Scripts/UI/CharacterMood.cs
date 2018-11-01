using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMood : MonoBehaviour
{

    private Character character;
    public RectTransform moodUi;
    public Transform canvas;
    private Text moodText;
    private Camera cam;



    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        character = GetComponent<Character>();
        moodUi = Instantiate(moodUi.gameObject, canvas).GetComponent<RectTransform>();
        moodText = moodUi.GetComponentInChildren<Text>();
        moodText.text = character.mood.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 viewPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 2.1f);
        moodUi.transform.position = viewPos;
        if (cam.pixelRect.Contains(viewPos))
        {
            if (!moodUi.gameObject.activeSelf)
            {
                moodUi.gameObject.SetActive(true);
            }

            moodUi.transform.localScale = Vector3.one * Mathf.Min(1, 30 / cam.fieldOfView);
            moodUi.transform.position = viewPos;
        }

        else
        {
            if (moodUi.gameObject.activeSelf)
            {
                moodUi.gameObject.SetActive(false);
            }
        }
    }
}