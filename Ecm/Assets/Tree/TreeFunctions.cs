using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Mtree
{
    
    [System.Serializable]
    public class TreeFunction // All Functions are implemented in one class since Unity doens't Serialize child classes
    {
        public int seed;
        public FunctionType type;
        public int creator;

        // Trunk parameters
        public float TradiusMultiplier = .3f;
        public AnimationCurve Tradius;
        public float Tlength = 10;
        public int Tnumber = 1;
        public float Trandomness = .1f;
        public float ToriginAttraction = .1f;
        public float Tresolution = 1.5f;
        public AnimationCurve TrootShape;
        public float TrootRadius = .25f;
        public float TrootHeight = 1f;
        public float TrootResolution = 3f;
        public int TflareNumber = 5;
        public float TspinAmount = .1f;
        public float TdisplacmentStrength = 1f;
        public float TdisplacmentSize = 2.5f;


        // Grow Parameters
        public float Glength = 10f;
        public AnimationCurve GlengthCurve = AnimationCurve.Linear(0, 1, 1, .8f);
        public float Gresolution = 1f;
        public float GsplitProba = .1f;
        public float GsplitAngle = .5f;
        public int GmaxSplits = 2;
        public AnimationCurve Gradius = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve GsplitProbaCurve = AnimationCurve.Linear(0, .5f, 1, 1f);
        public float GsplitRadius = .8f;
        public float Grandomness = .25f;
        public float GupAttraction = .5f;

        // Split Parameters
        public int Snumber = 15;
        public float SsplitAngle = .5f;
        public int SmaxSplits = 2;
        public float SsplitRadius = .6f;
        public float SstartLength = 2;
        public float Sspread = 1;

        // Branch Parameters
        public float Blength = 7f;
        public AnimationCurve BlengthCurve = AnimationCurve.Linear(0, 1, 1, .8f);
        public float Bresolution = 1f;
        public int Bnumber = 15;
        public float BsplitProba = .2f;
        public float Bangle = .7f;
        public float Brandomness = .3f;
        public AnimationCurve Bshape = AnimationCurve.Linear(0, 1, 1, 0);
        public AnimationCurve BsplitProbaCurve = AnimationCurve.Linear(0, .5f, 1, 1f);
        public float Bradius = .7f;
        public float BupAttraction = .7f;
        public float Bstart = 2f;
        public int BmaxSplits = 4;


        // Leaf Parameters
        public Mesh LleafMesh;
        public int Lnumber = 50;
        public float LmaxRadius = .1f;
        public float Lsize = 1f;
        public bool LoverrideNormals = true;
        public float LminWeight = 0f;
        public float LmaxWeight = 1f;
        
        public TreeFunction(int creator, FunctionType type)
        {
            this.creator = creator;
            this.type = type;
            seed = Random.Range(0, 1000);

            if (type == FunctionType.Branch && creator > 1)
            {
                Blength = 3;
                Bstart = 1f;
                Bangle = .5f;
                Bnumber = 55;
                BmaxSplits = 2;
                Bresolution = 3;
            }
            if (type == FunctionType.Trunk)
            {
                Keyframe[] keys = new Keyframe[2] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, -1f, -1f) };
                Tradius = new AnimationCurve(keys);
                Keyframe[] rootKeys = new Keyframe[2] { new Keyframe(0f, 1f, -2f, -2f), new Keyframe(1f, 0f, 0f, 0f) };
                TrootShape = new AnimationCurve(rootKeys);
            }
        }

        public void Execute(MTree tree, int selection)
        {
            Random.InitState(seed);

            if (type == FunctionType.Trunk)
                tree.AddTrunk(Vector3.zero, Vector3.up, Tlength, Tradius, TradiusMultiplier, Tresolution, Trandomness
                            , creator, TrootShape, TrootRadius, TrootHeight, TrootResolution, ToriginAttraction);


            if (type == FunctionType.Grow)
                tree.Grow(Glength, GlengthCurve, Gresolution, GsplitProba, GsplitProbaCurve, GsplitAngle, GmaxSplits, selection
                        , creator, Grandomness, Gradius, GsplitRadius, GupAttraction);

            if (type == FunctionType.Split)
                tree.Split(selection, Snumber, SsplitAngle, creator, SsplitRadius, SmaxSplits, SstartLength, Sspread);

            if (type == FunctionType.Branch)
                tree.AddBranches(selection, Blength, BlengthCurve, Bresolution, Bnumber, BsplitProba, BsplitProbaCurve, Bangle
                                , Brandomness, Bshape, Bradius, BupAttraction, creator, Bstart, BmaxSplits);

            if (type == FunctionType.Leaf)
                tree.AddLeafs(LmaxRadius, Lnumber, LleafMesh, Lsize, LoverrideNormals, LminWeight, LmaxWeight);
        }

    }

    public enum FunctionType {Trunk, Grow, Split, Branch, Leaf}

}