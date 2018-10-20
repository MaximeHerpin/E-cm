using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiantOcclusion : MonoBehaviour {

    public MeshFilter TargetObject;
    public Mesh target;
    public Camera cam;
    public int resolution = 25;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BakeAo()
    {
        target = TargetObject.sharedMesh;
        int n = target.vertexCount;
        Color[] vertexColors = new Color[n];

        for (int i=0; i<n; i++)
        {
            Vector3 pos = target.vertices[i];
            Vector3 dir = target.normals[i];
            Quaternion rot = Quaternion.LookRotation(dir);
            cam.transform.SetPositionAndRotation(pos, rot);
            float ao = BackgroundAmount(RenderCamera());
            vertexColors[i] = new Color(ao, ao, ao);
        }

        target.colors = vertexColors;
    }


    public Texture2D RenderCamera() //return an image of camera render texture 
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        return image;
    }

    public float BackgroundAmount(Texture2D render) //get the ratio of background colored pixels in an image
    {
        Color[] colors = render.GetPixels();
        Color background = cam.backgroundColor;
        int backgroundColoredPixelsNumber = 0;
        foreach(Color c in colors)
        {
            if (c.r > .9)
                backgroundColoredPixelsNumber++;
        }

        return ((float) backgroundColoredPixelsNumber) / colors.Length;
    }
}
