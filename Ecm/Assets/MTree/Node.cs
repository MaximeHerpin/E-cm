using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mtree
{

    public class Node
    {
        public List<Node> children;
        public Vector3 position;
        public Vector3 direction;
        public float radius;
        public int creator;
        public float distanceFromOrigin; // total branch distance from the root to the Node
        public NodeType type; // Used to displace the tree after Growth
        public GrowthInformation growth;
        

        public Node(Vector3 pos, float rad, Vector3 dir, int crea, NodeType type = NodeType.Branch, float distancToOrigin=0)
        {
            position = pos;
            radius = rad;
            direction = dir;
            children = new List<Node>();
            creator = crea;
            growth = new GrowthInformation(0, 0, 1);
            this.type = type;
            this.distanceFromOrigin = distancToOrigin;
        }

        public Queue<Node> Grow(float length, float splitProba, int maxNumber, float rad, float secondaryBranchRadius, float angle
                                , int creator, float randomness, float upAttraction, Transform treeTransform)
        {
            int number = (int) (splitProba / Random.value) + 1; // The number of branches that will be generated
            if (number > maxNumber)
                number = maxNumber;
            if (!growth.canBeSplit)
                number = 1;
            
            Queue<Node> extremities = new Queue<Node>();
            bool outOfLength = length > growth.remainingGrowth; // Is the growth ending
            if (outOfLength)
                length = growth.remainingGrowth;

            for (int i=0; i<number; i++)
            {
                Vector3 tangent = Vector3.Cross(Random.onUnitSphere, direction);
                if (tangent.y < 0)
                {
                    tangent.y -= tangent.y * upAttraction;
                }
                
                if (i == 0)
                {
                    Vector3 dir = (direction * (1 - randomness) + tangent * randomness).normalized;
                    if (ObstacleAvoidance(ref dir, position, treeTransform, 10))
                        continue;
                    Vector3 pos = position + dir * length;
                    Node child = new Node(pos, rad, dir, creator, distancToOrigin: distanceFromOrigin+(length/(.1f + radius)));
                    child.growth = new GrowthInformation(growth.growthGoal, growth.remainingGrowth - length, growth.initialRadius);
                    if(!outOfLength)
                        extremities.Enqueue(child);
                    children.Add(child);
                }
                else
                {
                    Vector3 dir = (direction * (1 - angle) + tangent * (angle)).normalized;
                    if (ObstacleAvoidance(ref dir, position, treeTransform, 10))
                        continue;
                    Vector3 pos = position + dir * length;
                    Node child0 = new Node(position, rad, dir, creator, distancToOrigin: distanceFromOrigin);
                    child0.growth.canBeSplit = false; // Make sure The start of the new branch can't be split.
                    Node child1 = new Node(pos, rad, dir, creator, distancToOrigin: distanceFromOrigin + length);
                    child0.children.Add(child1);
                    children.Add(child0);
                    child1.growth = new GrowthInformation(growth.growthGoal, growth.remainingGrowth - length, growth.initialRadius * secondaryBranchRadius);
                    if (!outOfLength)
                        extremities.Enqueue(child1);
                }

            }
            return extremities;
        }

        //public void Split(float splitProba, float angle, float branchRadius, int maxNumber, float length, int creator, float startLength
        //                , float remainingLength, float spread)
        //{
        //    float distToChild = 0f;
        //    if(children.Count > 0)
        //        distToChild = (position - children[0].position).magnitude;
        //    if (children.Count > 1 || remainingLength > 0 || !growth.canBeSplit)
        //    {
        //        int n = children.Count;
        //        for (int i=0; i<n; i++)
        //        {
        //            children[i].Split(splitProba, angle, branchRadius, maxNumber, length, creator, startLength, remainingLength - distToChild, spread);
        //        }
        //    }

        //    else if (children.Count == 1)
        //    {
        //        int number = (int)(splitProba / Random.value);
        //        if (number > maxNumber)
        //        {
        //            number = maxNumber;
        //        }

        //        if(number > 0)
        //        {
        //            Vector3 tangent = Vector3.Cross(Random.onUnitSphere, direction);
        //            Quaternion rot = Quaternion.AngleAxis(360f / number, direction);
        //            Vector3 spreadDirection = children[0].direction;
        //            for(int i=0; i<number; i++)
        //            {
        //                float blend = Random.value * spread;
        //                float rad = Mathf.Lerp(radius * branchRadius, children[0].radius * branchRadius, blend);
        //                Vector3 pos = position + spreadDirection * blend * distToChild;
        //                Vector3 dir = (angle * tangent + (1 - angle) * direction).normalized;
        //                Node child0 = new Node(pos, radius * .9f + rad * .1f, dir, creator, distancToOrigin: distanceFromOrigin+distToChild);
        //                Node child1 = new Node(pos + dir*radius * 1.5f, rad, dir, creator, distancToOrigin: distanceFromOrigin + distToChild);

        //                child0.children.Add(child1);

        //                child0.growth.canBeSplit = false;
        //                children.Add(child0);
        //                tangent = rot * tangent;
        //            }
        //        }
        //        children[0].Split(splitProba, angle, branchRadius, maxNumber, length, creator, startLength, remainingLength, spread);
        //    }
        //}



        public Stack<Queue<TreePoint>> ToSplines()
        {
            Stack<Queue<TreePoint>> points = new Stack<Queue<TreePoint>>();
            points.Push(new Queue<TreePoint>());
            this.ToSplinesRec(points, Vector3.up);
            return points;
        }

        private void ToSplinesRec(Stack<Queue<TreePoint>> points, Vector3 parentDirection)
        {
            points.Peek().Enqueue(new TreePoint(position, direction, radius, type, parentDirection, distanceFromOrigin));
            int n = children.Count;
            if (n > 0)
                children[0].ToSplinesRec(points, direction);
            for (int i=1; i<n; i++)
            {
                points.Push(new Queue<TreePoint>());
                children[i].ToSplinesRec(points, direction);
            }
        }

        public void GetSelection(Queue<Node> selected, int selection, bool extremitiesOnly, ref float maxHeight)
        {
            if (selection == creator && !(extremitiesOnly && children.Count > 0))
            {
                selected.Enqueue(this);
                if (position.y > maxHeight)
                    maxHeight = position.y;
            }
            foreach (Node child in children)
            {
                child.GetSelection(selected, selection, extremitiesOnly, ref maxHeight);
            }
        }
        
        public void GetSplitCandidates(Queue<Node> selected, int selection, float start, float remainingLength)
        {
            if (remainingLength <= 0 && selection == creator && children.Count == 1 && radius > .01f)
            {
                selected.Enqueue(this);
            }
            
            for(int i=0; i<children.Count; i++)
            {
                if (i == 0)
                {
                    float dist = (children[i].position - position).magnitude;
                    children[i].GetSplitCandidates(selected, selection, start, remainingLength - dist);
                }
                else
                {
                    children[i].GetSplitCandidates(selected, selection, start, start);
                }
            }
        }

        public void DebugPosRec(List<Vector3> positions)
        {
            positions.Add(position);
            foreach(Node child in children)
            {
                child.DebugPosRec(positions);
            }
        }

        public void AddGrowth(float growthLength)
        {
            growth.remainingGrowth = growthLength;
            growth.growthGoal = growthLength;
            growth.initialRadius = radius;
        }
        
        private bool ObstacleAvoidance(ref Vector3 dir, Vector3 position, Transform transform, float distance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.TransformPoint(position), transform.TransformDirection(dir), out hit, distance))
            {
                Vector3 disp = transform.InverseTransformDirection(hit.normal) * Mathf.Exp(-hit.distance * .3f) * 1;
                dir += disp;
                dir.Normalize();
                return disp.magnitude > .5f;
            }
            return false;
        }

        public void GetLeafCandidates(Queue<LeafCandidate> candidates, float maxBranchRadius, ref float totalLength)
        {
            if (radius < maxBranchRadius)
            {
                float branchLength = 0f;
                if (children.Count > 0)
                {
                    branchLength = (children[0].position - position).magnitude;
                    totalLength += branchLength;
                    candidates.Enqueue(new LeafCandidate(position, direction, Mathf.Pow(radius, .2f), branchLength, distanceFromOrigin));
                }
            }
            foreach (Node child in children)
            {
                child.GetLeafCandidates(candidates, maxBranchRadius, ref totalLength);
            }
        }

        public void Simplify(Node parent, float angleThreshold, float radiusThreshold)
        {
            if (radius < radiusThreshold)
            {
                children = new List<Node>();
                return;
            }

            Node nextParent = this;
            int n;
            if (children.Count > 0 && parent != null)
            {
                Vector3 v1 = position - parent.position;
                Vector3 v2 = children[0].position - position;
                if ((Vector3.Angle(v1, v2) < angleThreshold) && type != NodeType.Flare) // if true current Node must be removed
                {
                    List<Node> parentChildren = new List<Node>() {children[0]}; // new childern for parent, with first child being self first child
                    n = parent.children.Count;
                    for (int i = 1; i < n; i++) // adding  original parent children
                    {
                        parentChildren.Add(parent.children[i]);
                    }
                    n = children.Count;
                    for(int i=1; i<n; i++)
                    {
                        parentChildren.Add(children[i]); // adding self children except firt one whih is already in list
                    }                    
                    parent.children = parentChildren;
                    nextParent = parent;
                }
            }
            n = 0;
            foreach (Node child in children)
            {
                if (n == 0)
                {
                    child.Simplify(nextParent, angleThreshold, radiusThreshold);
                }
                else
                {
                    child.Simplify(null, angleThreshold, radiusThreshold);
                }
                n++;
            }
        }

        public void Move()
        {
            position += Vector3.left * 5;
            foreach(Node child in children)
            {
                child.Move();
            }
        }
    }


    public enum NodeType {Flare, Trunk, Branch}

    public struct GrowthInformation
    {
        public float remainingGrowth;
        public float growthGoal;
        public float initialRadius;
        public bool canBeSplit;

        public GrowthInformation(float goal = 0f, float remaining = 0f, float radius = 1f)
        {
            remainingGrowth = remaining;
            growthGoal = goal;
            initialRadius = radius;
            canBeSplit = true;
        }

        public float GrowthPercentage(float additionalLength = 0f)
        {
            if (growthGoal == 0)
                return 0;
            return 1 - (remainingGrowth - additionalLength) / growthGoal;
        }
    }
}
