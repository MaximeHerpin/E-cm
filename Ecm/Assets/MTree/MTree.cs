using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mtree
{
    public class MTree
    {
        public List<Node> stems;
        public Vector3[] verts;
        public Vector3[] normals;
        public Vector2[] uvs;
        public int[] triangles;
        public int[] leafTriangles;
        public Color[] colors;
        public Transform treeTransform;

        private Queue<LeafPoint> leafPoints;


        public MTree(Transform transform)
        {
            stems = new List<Node>();
            leafPoints = new Queue<LeafPoint>();
            treeTransform = transform;
        }


        public void AddTrunk(Vector3 position, Vector3 direction, float length, AnimationCurve radius, float radiusMultiplier, float resolution
                           , float randomness, int creator, AnimationCurve rootShape, float rootRadius, float rootHeight, float RootResolutionMultiplier
                           , float originAttraction)
        {
            float remainingLength = length;
            Node extremity = new Node(position, radius.Evaluate(0) * radiusMultiplier, direction, creator, NodeType.Flare);
            stems.Add(extremity);
            while (remainingLength > 0)
            {
                float res = resolution;
                NodeType type = NodeType.Trunk;
                Vector3 tangent = Vector3.Cross(direction, Random.onUnitSphere);
                if (length - remainingLength < rootHeight)
                {
                    tangent /= RootResolutionMultiplier * 2;
                    res *= RootResolutionMultiplier;
                    type = NodeType.Flare;
                }

                float branchLength = 1f / res;
                Vector3 dir = randomness * tangent + extremity.direction * (1 - randomness);
                Vector3 originAttractionVector = (position - extremity.position) * originAttraction;
                originAttractionVector.y = 0;
                dir += originAttractionVector;
                dir.Normalize();
                if (remainingLength <= branchLength)
                    branchLength = remainingLength;
                remainingLength -= branchLength;

                Vector3 pos = extremity.position + dir * branchLength;
                float rad = radius.Evaluate(1 - remainingLength / length) * radiusMultiplier;
                //if (length - remainingLength < rootHeight)
                //    rad += radiusMultiplier * rootRadius * rootShape.Evaluate((length - remainingLength) / rootHeight);
                Node child = new Node(pos, rad, dir, creator, type, distancToOrigin: length-remainingLength);
                extremity.children.Add(child);
                extremity = child;
            }
        }


        public void Grow(float length, AnimationCurve lengthCurve, float resolution, float splitProba, AnimationCurve probaCurve, float splitAngle, int maxSplits, int selection, int creator
                        , float randomness, AnimationCurve radius, float splitRadius, float upAttraction)
        {
            float maxHeight = 0;
            Queue<Node> extremities = GetSelection(selection, true, ref maxHeight);
            int n = extremities.Count;

            for(int i=0; i<n; i++) // Update Growth information for the extremities to grow
            {
                Node ext = extremities.Dequeue();

                float growthLength = length * lengthCurve.Evaluate(ext.position.y / maxHeight); // The length of this specific branch after growth
                ext.AddGrowth(growthLength);
                extremities.Enqueue(ext);
            }

            float branchLength = 1 / resolution;
            while (extremities.Count > 0)
            {
                Queue<Node> newExtremities = new Queue<Node>();
                while (extremities.Count > 0)
                {
                    Node extremity = extremities.Dequeue();

                    float rad = radius.Evaluate(extremity.growth.GrowthPercentage(branchLength)) * extremity.growth.initialRadius;
                    if (rad < .0001f)
                        rad = .0001f;
                    float extremitySplitProba = probaCurve.Evaluate(extremity.growth.GrowthPercentage()) * splitProba;
                    Queue<Node> ext = extremity.Grow(branchLength, extremitySplitProba, maxSplits, rad, splitRadius, splitAngle, creator, randomness, upAttraction, treeTransform);
                    foreach(Node child in ext)
                    {
                        newExtremities.Enqueue(child);
                    }
                    
                }
                extremities = newExtremities;
            }
        }




        public void Split(int selection, int expectedNumber, float splitAngle, int creator, float splitRadius, int splitsNumber, float startLength
                        , float spread)
        {
            //foreach(Node stem in stems)
            //{
            //    stem.Split(splitProba, splitAngle, splitRadius, splitsNumber, .1f, creator, startLength, startLength, spread);
            //}

            Queue<Node> extremities = GetSplitCandidates(selection, startLength);
            int n = extremities.Count;
            while (extremities.Count > 0)
            {
                float splitProba = expectedNumber * 1f / n;
                Node ext = extremities.Dequeue();
                float distToChild = 0f;

                distToChild = (ext.position - ext.children[0].position).magnitude;
                int number = (int)(splitProba / Random.value);
                if (number > splitsNumber)
                    number = splitsNumber;
                
                if (number > 0)
                {
                    Vector3 tangent = Vector3.Cross(Random.onUnitSphere, ext.direction);
                    Quaternion rot = Quaternion.AngleAxis(360f / number, ext.direction);
                    Vector3 spreadDirection = ext.children[0].direction;
                    for (int i = 0; i < number; i++)
                    {
                        float blend = Random.value * spread;
                        float rad = Mathf.Lerp(ext.radius * splitRadius, ext.children[0].radius * splitRadius, blend);
                        Vector3 pos = ext.position + spreadDirection * blend * distToChild;
                        Vector3 dir = (splitAngle * tangent + (1 - splitAngle) * ext.direction).normalized;
                        Node child0 = new Node(pos, ext.radius * .7f + rad * .3f, dir, creator, distancToOrigin: ext.distanceFromOrigin + blend * distToChild);
                        Node child1 = new Node(pos + dir * ext.radius * 1.5f, rad, dir, creator, distancToOrigin: ext.distanceFromOrigin + blend * distToChild);

                        child0.children.Add(child1);

                        child0.growth.canBeSplit = false;
                        ext.children.Add(child0);
                        tangent = rot * tangent;
                    }
                }
                
            }

        }


        public void AddBranches(int selection,  float length, AnimationCurve lengthCurve, float resolution, int number, float splitProba, AnimationCurve splitProbaCurve
                                , float angle, float randomness, AnimationCurve shape, float radius, float upAttraction, int creator
                                , float start, int MaxSplits)
        {
            Split(selection, number, angle, creator, radius, MaxSplits, start, 1);
            Grow(length, lengthCurve, resolution, splitProba, splitProbaCurve, .3f, 2, creator, creator, randomness, shape, .9f, upAttraction);
        }


        public void AddLeafs(float maxRadius, int number, Mesh mesh, float size, bool overrideNormals, float minWeight, float maxWeight)
        {
            Queue<LeafCandidate> candidates = new Queue<LeafCandidate>();
            float totalLength = 0f;
            foreach (Node stem in stems)
                stem.GetLeafCandidates(candidates, maxRadius, ref totalLength);
            if (number < 1)
                number = 2;
            float leafSpacing = totalLength / number;
            float currentSpace = 0f;
            while (candidates.Count > 0)
            {
                LeafCandidate candidate = candidates.Dequeue();
                candidate.direction.y /= 3;
                while (currentSpace < candidate.parentBranchLength)
                {
                    Vector3 dir = Vector3.Lerp(candidate.direction, Vector3.down, Random.Range(minWeight, maxWeight));
                    Quaternion rot = Quaternion.Euler(0, Random.Range(-90, 90), 0);
                    dir = rot * dir;
                    leafPoints.Enqueue(new LeafPoint(candidate.position, dir, candidate.size * size, mesh, overrideNormals, candidate.distanceFromOrigin + currentSpace));
                    currentSpace += leafSpacing;
                }
                currentSpace -= candidate.parentBranchLength;
            }
        }


        private Queue<Node> GetSelection(int selection, bool extremitiesOnly, ref float maxHeight)
        {
            Queue<Node> selected = new Queue<Node>();
            foreach (Node stem in stems)
            {
                stem.GetSelection(selected, selection, extremitiesOnly, ref maxHeight);
            }
            return selected;
        }

        
        private Queue<Node> GetSplitCandidates(int selection, float start)
        {
            Queue<Node> selected = new Queue<Node>();
            foreach (Node stem in stems)
            {
                stem.GetSplitCandidates(selected, selection, start, start);
            }
            return selected;
        }


        public void Simplify(float angleThreshold, float radiusTreshold)
        {
            foreach(Node stem in stems)
            {
                stem.Simplify(null, angleThreshold, radiusTreshold);
            }
        }


        public void GenerateMeshData(TreeFunction trunk, float simplifyLeafs, float radialResolution)
        {
            Stack<Queue<TreePoint>> treePoints = new Stack<Queue<TreePoint>>();
            foreach (Node stem in stems)
            {
                Stack<Queue<TreePoint>> newPoints = stem.ToSplines();
                while(newPoints.Count > 0)
                {
                    treePoints.Push(newPoints.Pop());
                }
            }
            Splines splines = new Splines(treePoints);
            splines.GenerateMeshData(7*radialResolution, 3, trunk.TrootShape, trunk.TradiusMultiplier, trunk.TrootRadius, trunk.TrootHeight, trunk.TrootResolution
                                    , trunk.TflareNumber, trunk.TdisplacmentStrength, trunk.TdisplacmentSize, trunk.TspinAmount);


            Queue<int> leafTriangles = new Queue<int>();
            GenerateLeafData(splines.verts, splines.normals, splines.uvs, leafTriangles, splines.verts.Count, splines.colors, simplifyLeafs);
            verts = splines.verts.ToArray();
            normals = splines.normals.ToArray();
            uvs = splines.uvs.ToArray();
            triangles = splines.triangles.ToArray();
            colors = splines.colors.ToArray();
            this.leafTriangles = leafTriangles.ToArray();
        }


        public void GenerateLeafData(Queue<Vector3> leafVerts, Queue<Vector3> leafNormals, Queue<Vector2> leafUvs, Queue<int> leafTriangles, int vertexOffset, Queue<Color> colors, float simplify)
        {
            foreach(LeafPoint l in leafPoints)
            {
                if (l.mesh == null || Random.value < simplify) // don't create the leaf when no object is available or when simplifying
                    continue;

                int n = l.mesh.vertexCount;
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, l.direction);
                rot = Quaternion.LookRotation(l.direction);
                float randomWindPhase = Random.value; // used is wind shader to randomise vertex displacement phase
                for (int i=0; i<n; i++) // iterating over leaf object vertices
                {
                    Vector3 vert = l.mesh.vertices[i];
                    leafVerts.Enqueue(rot * vert * l.size * (1+simplify*.5f) + l.position);
                    if (l.overrideNormals)
                    {
                        leafNormals.Enqueue(l.position.normalized);
                    }
                    else
                    {
                        leafNormals.Enqueue(l.mesh.normals[i]);
                    }
                    leafUvs.Enqueue(l.mesh.uv[i]);
                    float blueChannel = 1;
                    if (Vector3.Distance(l.position, vert) < .1f) // the vertices near the attachement point have b=0 so that the shader won't move them
                        blueChannel = 0;
                    colors.Enqueue(new Color(l.distanceFromOrigin/10, randomWindPhase, blueChannel));
                }
                int m = l.mesh.triangles.Length;
                for (int i = 0; i < m; i++) // creating the triangles
                {
                    leafTriangles.Enqueue(l.mesh.triangles[i] + vertexOffset);
                }
                vertexOffset += n; // used when creating submeshes for the mesh component
            }
        }


        public List<Vector3> DebugPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach(Node stem in stems)
            {
                stem.DebugPosRec(positions);
            }
            return positions;
        }
    }

}
