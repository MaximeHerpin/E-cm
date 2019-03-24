using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler {
    Vector2 lastMousePos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 mouseDelta = mousePos - lastMousePos;
        transform.position += mouseDelta;

        lastMousePos = mousePos;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
