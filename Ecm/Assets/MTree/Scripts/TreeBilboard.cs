#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mtree
{
    class Billboard
    {
        private Camera cam;
        private GameObject target;
        private int width = 512;
        private int height = 512;
        private Texture2D[] sides;
        private Rect[] rects;

        
        public Billboard(Camera cam, GameObject target, int width, int height)
        {
            this.cam = cam;
            this.target = target;
            this.width = width;
            this.height = height;
            sides = new Texture2D[4];
        }

        public void SetupCamera()
        {
            float rw = width;
            rw /= Screen.width;
            float rh = height;
            rh /= Screen.height;
            //cam.rect = new Rect(0, 0, rw, rh);

            Bounds bb = target.GetComponent<Renderer>().bounds;

            cam.transform.position = bb.center;
            //cam.clearFlags = CameraClearFlags.SolidColor;
            //cam.backgroundColor = new Color(0, 0, 0, 0);
            //cam.transform.position.Set(cam.transform.position.x, cam.transform.position.y, -1.0f + (bb.min.z * 2.0f));
            //make clip planes fairly optimal and enclose whole mesh
            cam.nearClipPlane = - bb.extents.z;
            cam.farClipPlane =  bb.extents.z;

            cam.orthographicSize = 1.01f * Mathf.Max((bb.max.y - bb.min.y) / 2.0f, (bb.max.x - bb.min.x) / 2.0f);
            cam.transform.position.Set(cam.transform.position.x, cam.orthographicSize * 0.05f, cam.transform.position.y);

        }

        public void Render(string path)
        {
            int layer = target.layer;
            target.layer = 31;
            cam.cullingMask = 1 << 31;
            for (int i = 0; i < 4; i++)
            {
                RenderTexture currentRT = RenderTexture.active;
                RenderTexture camText = new RenderTexture(width, height, 16);
                cam.targetTexture = camText;
                RenderTexture.active = cam.targetTexture;
                cam.Render();
                Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
                image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
                image.Apply();
                RenderTexture.active = currentRT;
                sides[i] = image;
                cam.transform.Rotate(Vector3.up * 90);
            }

            Texture2D atlas = new Texture2D(512, 512);
            rects = atlas.PackTextures(sides, 0, 1024);
            Utils.DilateTexture(atlas, 100);
            byte[] bytes = atlas.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            target.layer = layer;
        }

        public Mesh CreateMesh()
        {
            Vector3[] verts = new Vector3[16];
            Vector3[] normals = new Vector3[16];
            Vector2[] uvs = new Vector2[16];
            int[] triangles = new int[24];

            Bounds bb = target.GetComponent<MeshRenderer>().bounds;

            for (int i=0; i<4; i++)
            {
                float angle = 90f * i + 180;
                Vector3 x = Quaternion.Euler(0f, angle, 0f) * Vector3.left * bb.extents.x;
                Vector3 normal = Quaternion.Euler(0f, angle, 0f) * Vector3.back;
                Vector3 y = Vector3.up * bb.extents.y * 2;

                verts[4*i] = x;
                verts[4 * i + 1] = x + y;
                verts[4 * i + 2] = y - x;
                verts[4 * i + 3] = -x;

                x.Normalize();
                float normalFacing =50f;
                normals[4 * i] = (x + normalFacing * normal).normalized;
                normals[4 * i + 1] = (x + normalFacing * normal).normalized;
                normals[4 * i + 2] = (-x + normalFacing * normal).normalized;
                normals[4 * i + 3] = (-x + normalFacing * normal).normalized;

                triangles[6 * i] = 4 * i;
                triangles[6 * i + 1] = 4 * i + 1;
                triangles[6 * i + 2] = 4 * i + 2;
                triangles[6 * i + 3] = 4 * i + 2;
                triangles[6 * i + 4] = 4 * i + 3;
                triangles[6 * i + 5] = 4 * i;

                Vector2 c = rects[i].center;
                float scaleX = bb.extents.x / bb.extents.y;
                Vector2 u = new Vector2(rects[i].max.x - c.x, 0f) * scaleX;
                Vector2 v = new Vector2(0f, rects[i].max.y - c.y);
                uvs[4 * i] = c + u - v;
                uvs[4 * i + 1] = c + u + v;
                uvs[4 * i + 2] = c - u + v;
                uvs[4 * i + 3] = c - u - v;
            }
            Mesh mesh = new Mesh
            {
                vertices = verts,
                triangles = triangles,
                uv = uvs,
                normals = normals
            };
            return mesh;
        }

        public Material CreateMaterial(Texture tex)
        {
            Bounds bb = target.GetComponent<MeshRenderer>().bounds;
            var pipeline = GraphicsSettings.renderPipelineAsset;
            Shader shader = Utils.GetBillboardShader();
            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", tex);
            return mat;
        }

    }

}
#endif