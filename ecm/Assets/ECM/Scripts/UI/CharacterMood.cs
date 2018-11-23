using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMood : MonoBehaviour
{

    private Character character;
    public RectTransform moodUi;
    public RectTransform canvas;
    public float uiStart = 7;
    private Text moodText;
    private Camera cam;
    private Animator moodAnimator;
    private bool isFading = false;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        character = GetComponent<Character>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        moodUi = Instantiate(moodUi.gameObject, canvas).GetComponent<RectTransform>();
        moodText = moodUi.GetComponentInChildren<Text>();
        moodText.text = character.realName;
        moodAnimator = moodUi.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.fieldOfView < (uiStart + transform.position.y / 2) || isFading)
        {
            Vector2 viewPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 2.1f);
            moodUi.transform.position = viewPos;
            if (cam.pixelRect.Contains(viewPos))
            {
                if (!moodUi.gameObject.activeSelf)
                {
                    moodUi.gameObject.SetActive(true);
                }

                //moodUi.transform.localScale = Vector3.one * Mathf.Min(1, 30 / cam.fieldOfView);
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
        else
        {
            if (moodUi.gameObject.activeSelf && ! isFading)
            {
                isFading = true;
                moodAnimator.SetTrigger("Fade");
                Invoke("DeactivateMood", .2f * Time.timeScale);
                //moodUi.gameObject.SetActive(false);
            }
        }
    }

    private void DeactivateMood()
    {
        isFading = false;
        moodUi.gameObject.SetActive(false);
    }
}