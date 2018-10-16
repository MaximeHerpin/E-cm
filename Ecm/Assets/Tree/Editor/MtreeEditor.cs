using UnityEngine;
using UnityEditor;
using Mtree;

[CustomEditor(typeof(MtreeComponent))]
public class MtreeEditor: Editor
{
    MtreeComponent tree;
    private string[] functionNames; //names displayed on the interface on the function list
    private string[] lodOptions = { "0", "1", "2", "3" };

    private void OnEnable()
    {
        tree = (MtreeComponent)target;
        if (tree.tree == null)
        {
            UpdateTree();
        }
        UpdateFunctionNames(updateTree:false);
        //tree.GenerateTree();
        //UpdateFunctionNames();
    }

    private void UpdateFunctionNames(bool updateTree=true)
    {
        int n = tree.treeFunctions.Count;
        functionNames = new string[n];
        for (int i=0; i<n; i++)
        {
            functionNames[i] = tree.treeFunctions[i].type.ToString();
        }
        if (updateTree)
            UpdateTree();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        tree.Lod = EditorGUILayout.Popup("LOD", tree.Lod, lodOptions);
        if (EditorGUI.EndChangeCheck()){
            UpdateTree();
        }

        // Add Function buttons -------------------------------------------
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Branches"))
        {
            tree.AddBranchFunction();
            tree.selectedFunction = tree.treeFunctions.Count - 1;
            UpdateFunctionNames();
        }
        if (GUILayout.Button("Grow"))
        {
            tree.AddGrowFunction();
            tree.selectedFunction = tree.treeFunctions.Count - 1;
            UpdateFunctionNames();
        }
        if (GUILayout.Button("Split"))
        {
            tree.AddSplitFunction();
            tree.selectedFunction = tree.treeFunctions.Count - 1;
            UpdateFunctionNames();
        }
        if (GUILayout.Button("Leafs"))
        {
            tree.AddLeafFunction();
            tree.selectedFunction = tree.treeFunctions.Count - 1;
            UpdateFunctionNames();
        }
        EditorGUILayout.EndHorizontal();

        // Applied functions List -----------------------------------------
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        int n = tree.treeFunctions.Count;
        tree.selectedFunction = GUILayout.SelectionGrid(tree.selectedFunction, functionNames, 1
                                                        , GUILayout.Width(EditorGUIUtility.currentViewWidth * 3f / 4 - 15));
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < tree.treeFunctions.Count; i++) // delete buttons
        {
            if (i == 0) // First function should not be deleted
            {
                GUILayout.Space(21);
            }
            else if (GUILayout.Button("delete", GUILayout.Width(EditorGUIUtility.currentViewWidth / 4 - 31)) && tree.treeFunctions.Count > 0)
            {
                tree.RemoveFunction(i);
                UpdateFunctionNames();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Mtree.TreeFunction f = tree.treeFunctions[tree.selectedFunction];

        if(f.type == FunctionType.Trunk)
        {
            
            f.seed = EditorGUILayout.IntField("Seed", f.seed);
            f.Tlength = EditorGUILayout.FloatField("Length", f.Tlength);
            f.Tlength = Mathf.Max(0.01f, f.Tlength);
            f.Tresolution = EditorGUILayout.FloatField("Resolution", f.Tresolution);
            f.Tresolution = Mathf.Max(.01f, f.Tresolution);
            f.ToriginAttraction = EditorGUILayout.Slider("Axis attraction", f.ToriginAttraction, 0, 1);
            f.Tradius = EditorGUILayout.CurveField("Shape", f.Tradius);
            f.TradiusMultiplier = EditorGUILayout.FloatField("Radius multiplier", f.TradiusMultiplier);
            f.TradiusMultiplier = Mathf.Max(0.0001f, f.TradiusMultiplier);
            f.Trandomness = EditorGUILayout.Slider("Randomness", f.Trandomness, 0f, 3f);
            f.TdisplacmentStrength = EditorGUILayout.FloatField("Displacment strength", f.TdisplacmentStrength);
            f.TdisplacmentSize = EditorGUILayout.FloatField("Displacment size", f.TdisplacmentSize);
            f.TspinAmount = EditorGUILayout.FloatField("Spin amount", f.TspinAmount);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Root");
            f.TrootShape = EditorGUILayout.CurveField("Root shape", f.TrootShape);
            f.TrootHeight = EditorGUILayout.FloatField("Height", f.TrootHeight);
            f.TrootRadius = EditorGUILayout.FloatField("Radius", f.TrootRadius);
            f.TrootResolution = EditorGUILayout.FloatField("Additional Resolution", f.TrootResolution);
            f.TflareNumber = EditorGUILayout.IntField("Flare Number", f.TflareNumber);
            EditorGUILayout.EndVertical();

        }

        else if (f.type == FunctionType.Grow)
        {
            f.seed = EditorGUILayout.IntField("Seed", f.seed);
            EditorGUILayout.BeginHorizontal();
            f.Glength = EditorGUILayout.FloatField("Length", f.Glength);
            f.GlengthCurve = EditorGUILayout.CurveField(f.GlengthCurve);
            EditorGUILayout.EndHorizontal();
            f.Glength = Mathf.Max(.001f, f.Glength);
            f.Gresolution = EditorGUILayout.FloatField("Resolution", f.Gresolution);
            EditorGUILayout.BeginHorizontal();
            f.GsplitProba = EditorGUILayout.Slider("Split proba", f.GsplitProba, 0, 1);
            f.GsplitProbaCurve = EditorGUILayout.CurveField(f.GsplitProbaCurve);
            EditorGUILayout.EndHorizontal();
            f.GsplitAngle = EditorGUILayout.Slider("Split angle", f.GsplitAngle, 0, 2);
            f.Gradius = EditorGUILayout.CurveField("Shape", f.Gradius);
            f.GsplitRadius = EditorGUILayout.Slider("Split radius", f.GsplitRadius, .5f, .999f);
            f.GmaxSplits = EditorGUILayout.IntSlider("Max splits at a time", f.GmaxSplits, 2, 4);
            f.Grandomness = EditorGUILayout.Slider("Randomness", f.Grandomness, 0, 1);
            f.GupAttraction = EditorGUILayout.Slider("Up attraction", f.GupAttraction, 0, 1f);
        }

        else if (f.type == FunctionType.Split)
        {
            f.seed = EditorGUILayout.IntField("Seed", f.seed);
            f.Snumber = EditorGUILayout.IntField("Number", f.Snumber);
            f.SsplitAngle = EditorGUILayout.Slider("Split angle", f.SsplitAngle, 0, 2);
            f.SmaxSplits = EditorGUILayout.IntSlider("Max splits at a time", f.SmaxSplits, 1, 10);
            f.SsplitRadius = EditorGUILayout.Slider("split radius", f.SsplitRadius, 0.001f, 1);
            f.SstartLength = EditorGUILayout.FloatField("Start", f.SstartLength);
            f.Sspread = EditorGUILayout.Slider("Height spread", f.Sspread, 0, 1);
        }

        else if (f.type == FunctionType.Branch)
        {
            f.seed = EditorGUILayout.IntField("Seed", f.seed);
            f.Bnumber = EditorGUILayout.IntField("Number", f.Bnumber);
            EditorGUILayout.BeginHorizontal();
            f.Blength = EditorGUILayout.FloatField("Length", f.Blength);
            f.BlengthCurve = EditorGUILayout.CurveField(f.BlengthCurve);
            EditorGUILayout.EndHorizontal();
            f.Blength = Mathf.Max(f.Blength, .001f);
            f.Bresolution = EditorGUILayout.FloatField("Resolution", f.Bresolution);
            f.Bresolution = Mathf.Max(f.Bresolution, .01f);
            f.Brandomness = EditorGUILayout.Slider("Randomness", f.Brandomness, 0, 1);
            f.Bradius = EditorGUILayout.Slider("Radius", f.Bradius, 0.001f, 1);
            EditorGUILayout.BeginHorizontal();
            f.BsplitProba = EditorGUILayout.Slider("Split proba", f.BsplitProba, 0, 1);
            f.BsplitProbaCurve = EditorGUILayout.CurveField(f.BsplitProbaCurve);
            EditorGUILayout.EndHorizontal();
            f.BmaxSplits = EditorGUILayout.IntSlider("Max splits number", f.BmaxSplits, 1, 5);
            f.Bangle = EditorGUILayout.Slider("Angle", f.Bangle, 0, 2);
            f.BupAttraction = EditorGUILayout.Slider("Up attraction", f.BupAttraction, 0, 1);
            f.Bstart = EditorGUILayout.FloatField("Start", f.Bstart);
        }

        else if (f.type == FunctionType.Leaf)
        {
            f.LleafMesh =(Mesh) EditorGUILayout.ObjectField("Mesh: ", f.LleafMesh, typeof(Mesh), false);
            f.Lnumber = EditorGUILayout.IntField("number", f.Lnumber);
            f.Lsize = EditorGUILayout.FloatField("Size", f.Lsize);
            f.LmaxRadius = EditorGUILayout.Slider("Max branch radius", f.LmaxRadius, 0, 1);
            EditorGUILayout.MinMaxSlider("leafs weight", ref f.LminWeight, ref f.LmaxWeight, -1, 1);
            f.LoverrideNormals = EditorGUILayout.Toggle("Override Normals", f.LoverrideNormals);
        }

        int LOD = tree.Lod;
        tree.radialResolution[LOD] = EditorGUILayout.FloatField("Radial Resolution", tree.radialResolution[LOD]);
        tree.simplifyAngleThreshold[LOD] = EditorGUILayout.Slider("Simplify angle", tree.simplifyAngleThreshold[LOD], 0, 90);
        tree.simplifyRadiusThreshold[LOD] = EditorGUILayout.Slider("Simplify radius", tree.simplifyRadiusThreshold[LOD], 0, .1f);
        tree.simplifyLeafs[LOD] = EditorGUILayout.Slider("Simplify leafs", tree.simplifyLeafs[LOD], 0, 1f);


        EditorGUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck())
        {
            UpdateTree();
        }
        if (GUILayout.Button("Bake ambient occlusion"))
        {
            tree.BakeAo();
        }
        if (GUILayout.Button("Save as Prefab"))
        {
            tree.SaveAsPrefab();
        }

    }
    
    private void UpdateTree()
    {
        tree.GenerateTree();
    }
}
