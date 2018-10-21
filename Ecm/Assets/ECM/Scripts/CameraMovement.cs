using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private Vector3 up;
    private Vector3 side;
    private float currentZoom;
    public enum CameraMode {perspective, orthographic};
    public CameraMode cameraMode = CameraMode.orthographic;
    public float zoomMin = 5f;
    public float zoomMax = 15f;
    public float speed = 5f;
    public float zoomSpeed = 1f;
    public float rotSpeed = 1f;

    // The gamer can change the speed of all the different axis
    Camera cameraComponent;

	// Use this for initialization
	void Start () {
        cameraComponent = GetComponent<Camera>();
        currentZoom = cameraComponent.orthographicSize;
        side = transform.right; // Vector used to pan the camera sideways
        up = Vector3.Cross(side, Vector3.up); // Vector used to pan the camera upwards
        cameraComponent.orthographic = cameraMode == CameraMode.orthographic;
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 move = Vector3.zero;
        // To be sure that Vector3 is a vector of zeros

        float forwardMotion = Input.GetAxis("CameraVertical");
        float sideMotion = Input.GetAxis("CameraHorizontal");
        // We have set four new axis (CameraVertical, CameraHorizontal, CameraZoom) in Unity (Edit -> Project settings -> Input)
        transform.position += (up * forwardMotion + side * sideMotion) * Time.unscaledDeltaTime * speed * currentZoom;

        float zoom = Input.GetAxis("CameraZoom");

        if (zoom != 0) 
        {
            float newZoomValue;
            if (cameraMode == CameraMode.orthographic)
            {
                newZoomValue = cameraComponent.orthographicSize * (1 +zoom * zoomSpeed * Time.unscaledDeltaTime);
            }
            else
            {
                newZoomValue = cameraComponent.fieldOfView * (1 + zoom * zoomSpeed);
            }
            if (zoomMin <= newZoomValue && newZoomValue <= zoomMax)
            {
                if (cameraMode == CameraMode.orthographic)
                {
                    cameraComponent.orthographicSize = newZoomValue;
                }
                else
                {
                    cameraComponent.fieldOfView = newZoomValue;
                }
                currentZoom = newZoomValue;
            }
        }

        float rotationMotion = Input.GetAxis("CameraRotation");
        if(rotationMotion != 0)
        {
            float h = VisibilityManager.instance.level * 4;
            float cx = transform.position.x;
            float cy = transform.position.y;
            float cz = transform.position.z;
            Vector3 abc = transform.forward;
            float t = (h - cy) / abc.y;
            Vector3 rotationPoint = new Vector3(cx + t * abc.x, 0, cz + t * abc.z);
            transform.RotateAround(rotationPoint , Vector3.up, rotSpeed * Time.deltaTime * rotationMotion);
            side = transform.right;
            up = Vector3.Cross(side, Vector3.up);
        }

        
    }
}
