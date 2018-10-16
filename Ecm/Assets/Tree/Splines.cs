using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mtree
{
    public class Splines
    {
        Stack<Queue<TreePoint>> splines;
        public Queue<Vector3> verts;
        public Queue<Vector2> uvs;
        public Queue<Vector3> normals;
        public Queue<int> triangles;
        public Queue<Color> colors;

        public Splines(Stack<Queue<TreePoint>> points)
        {
            splines = points;
            verts = new Queue<Vector3>();
            uvs = new Queue<Vector2>();
            normals = new Queue<Vector3>();
            triangles = new Queue<int>();
            colors = new Queue<Color>();
        }

        public void GenerateMeshData(float resolutionMultiplier, int minResolution, AnimationCurve rootShape, float radiusMultiplier
                                    , float rootRadius, float rootHeight, float RootResolutionMultiplier, int flaresNumber, float displacmentStrength
                                    , float displacmentSize, float spinAmount)
        {
            Queue<Vector2Int> duplicatedVertexIndexes = new Queue<Vector2Int>(); //used when recalculating normals to avoid shading seams
            while (splines.Count > 0) // Each spline is an entire branch
            {
                Queue<TreePoint> spline = splines.Pop();
                int lastResolution = -1;
                float uv_height = 0f;

                SimplexNoiseGenerator noiseGenerator = new SimplexNoiseGenerator(new int[] { 0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a });
                while (spline.Count > 0) // drawing each node inside the branch
                {
                    TreePoint point = spline.Dequeue();
                    if (point.radius < .001f)
                        point.radius = .001f;
                    int resolution = (int)((point.radius) * resolutionMultiplier * 7);
                    
                    if (point.type == NodeType.Flare)
                    {
                        resolution = (int)((point.radius*(1+rootRadius * 2)) * resolutionMultiplier * 7);
                    }
                    if (resolution < minResolution)
                        resolution = minResolution;
                    resolution++;

                    int gaps = lastResolution - resolution; //difference between the two loops

                    int fillingGapRate = int.MaxValue; //rate at which an additional triangle must be created
                    if (gaps > resolution)
                        resolution = gaps; // increase resolution when there are too namy gaps to fill
                    if (gaps > 0)
                        fillingGapRate = resolution / gaps;
                    Vector3[] newVerts = AddCircle(point.position, point.direction, point.radius, resolution, uv_height*spinAmount * point.radius);
                    int n = verts.Count;
                    duplicatedVertexIndexes.Enqueue(new Vector2Int(n, n + resolution-1));
                    if (n > 65300)
                        return;
                    for (int i=0; i<resolution; i++)
                    {
                        Vector3 vert = newVerts[i];
                        Vector3 normal = (vert - point.position).normalized;

                        if (point.type == NodeType.Flare) // root flares displacement
                        {                            
                            float angle = i * 1f / (resolution-1) * 2 * Mathf.PI;
                            float displacement = Mathf.Abs(Mathf.Sin(angle * flaresNumber / 2f)) * rootRadius * rootShape.Evaluate(point.position.y / rootHeight);
                            vert += normal * displacement;
                        }

                        if(point.type == NodeType.Trunk || point.type == NodeType.Flare) //Trunk displacement
                            vert += noiseGenerator.noiseGradient(vert * displacmentSize, flat : true) / 5 * point.radius * displacmentStrength;

                        verts.Enqueue(vert);
                        Vector3 nrm = (vert - point.position).normalized;
                        if (lastResolution == -1)
                        {
                            float blend = Mathf.Abs(Vector3.Dot(point.parentDirection, nrm));
                            nrm = Vector3.Lerp(nrm, Vector3.Project(point.direction, point.parentDirection), blend).normalized;
                        }
                        normals.Enqueue(nrm);
                        uvs.Enqueue(new Vector2(i * 1f/(resolution-1) ,uv_height / 3.2f));
                        if (i > 0 && lastResolution > 0)
                        {
                            AddTriangle(n - lastResolution + i - 1, n + i - 1, n - lastResolution + i);
                            AddTriangle(n + i - 1, n + i, n - lastResolution + i);
                            if(i % fillingGapRate == 0 && gaps > 0) // filling a gap
                            {
                                AddTriangle(n - lastResolution + i, n + i, n - lastResolution + i + 1);
                                gaps--;
                                lastResolution--;
                            }
                        }
                        Color col = new Color(point.distanceFromOrigin / 10, 0, 0);
                        colors.Enqueue(col);
                    }
                    if (spline.Count > 0)
                        uv_height += (point.position - spline.Peek().position).magnitude / point.radius;
                    if (gaps > 0) // Fill eventual remaining gap
                        AddTriangle(n - lastResolution + resolution - 1, n + resolution - 1, n - lastResolution + resolution);
                    
                    lastResolution = resolution;
                }
            }

            RecalculateNormals(duplicatedVertexIndexes);
        }


        public void RecalculateNormals(Queue<Vector2Int> duplicatedVerts)
        {
            Vector3[] newNormals = new Vector3[normals.Count];
            Vector3[] Verts = verts.ToArray();
            int[] Tris = triangles.ToArray();
            int n = Tris.Length;
            for(int i=0; i<n; i+=3)
            {
                int a = Tris[i];
                int b = Tris[i+1];
                int c = Tris[i+2];

                Vector3 nrm = Vector3.Cross(Verts[b] - Verts[a], Verts[c] - Verts[a]);

                newNormals[a] += nrm;
                newNormals[b] += nrm;
                newNormals[c] += nrm;
            }

            foreach(Vector3 v in newNormals)
            {
                v.Normalize();
            }

            foreach (Vector2Int indexes in duplicatedVerts)
            {
                int x = indexes.x;
                int y = indexes.y;
                Vector3 nrm = (newNormals[x] + newNormals[y]) / 2;
                newNormals[x] = newNormals[y] = nrm;
            }

            normals = new Queue<Vector3>(newNormals);
        }

        public static Vector3[] AddCircle(Vector3 position, Vector3 direction, float radius, int resolution, float spinAngle)
        {
            Vector3[] verts = new Vector3[resolution];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);
            for (int i = 0; i < resolution; i++)
            {
                float angle = Mathf.PI * 2 * ((float)i / (resolution - 1)) + spinAngle;
                Vector3 vert = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                verts[i] = rotation * vert + position;
            }
            return verts;
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles.Enqueue(a);
            triangles.Enqueue(b);
            triangles.Enqueue(c);
        }
    }

    
}
