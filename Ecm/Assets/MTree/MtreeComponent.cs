using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mtree;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MtreeComponent : MonoBehaviour {

    public List<Mtree.TreeFunction> treeFunctions;
    public int selectedFunction = 0; // Index of selected Function in editor
    public MTree tree;
    public int Lod = 0;
    public float[] radialResolution = {1, .5f, .25f, .1f};
    public float[] simplifyAngleThreshold = { 3, 10, 20, 30 };
    public float[] simplifyRadiusThreshold = {0, .01f, .05f, .1f};
    public float[] simplifyLeafs = { 0, .2f, .4f, .6f };
    private int treeFunctionId = 0; // Id for the next created Function

	void InitializeTree()
    {
        tree = new MTree(transform);
    }

    public void AddTrunkFunction()
    {
        TreeFunction f = new TreeFunction(treeFunctionId, FunctionType.Trunk);
        treeFunctions.Add(f);
        treeFunctionId++;
    }

    public void AddGrowFunction()
    {
        TreeFunction f = new TreeFunction(treeFunctionId, FunctionType.Grow);
        treeFunctions.Add(f);
        treeFunctionId++;
    }

    public void AddSplitFunction()
    {
        TreeFunction f = new TreeFunction(treeFunctionId, FunctionType.Split);
        treeFunctions.Add(f);
        treeFunctionId++;
    }

    public void AddBranchFunction()
    {
        TreeFunction f = new TreeFunction(treeFunctionId, FunctionType.Branch);
        treeFunctions.Add(f);
        treeFunctionId++;
    }

    public void AddLeafFunction()
    {
        TreeFunction f = new TreeFunction(treeFunctionId, FunctionType.Leaf);
        treeFunctions.Add(f);
        treeFunctionId++;
    }

    public void RemoveFunction(int index)
    {
        Mtree.TreeFunction functionToRemove = treeFunctions[index];
        treeFunctions.RemoveAt(index);
        for (int i=index; i<treeFunctions.Count; i++)
        {
            treeFunctions[i].creator--;
        }
        if (selectedFunction >= index)
            selectedFunction--;
        treeFunctionId--;
    }

    private void ExecuteFunctions()
    {
        if (tree == null)
            InitializeTree();
        if (treeFunctions == null)
        {
            treeFunctions = new List<Mtree.TreeFunction>();
            AddTrunkFunction();
        }
        int n = treeFunctions.Count;
        for(int i=0; i<n; i++)
        {
            int selection = 0;
            if (i > 0)
                selection = treeFunctions[i - 1].creator;
            treeFunctions[i].Execute(tree, selection);
        }
    }

    Mesh CreateMesh(bool ao)
    {
        Mesh mesh = new Mesh();
        if (treeFunctions.Count > 0)
            tree.GenerateMeshData(treeFunctions[0], simplifyLeafs[Lod], radialResolution[Lod]);
        mesh.vertices = tree.verts;
        mesh.normals = tree.normals;
        mesh.uv = tree.uvs;
        Color[] colors = tree.colors;
        mesh.triangles = tree.triangles;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(tree.leafTriangles, 1);

        if (ao)
        {
            Ao.BakeAo(ref colors, tree.verts, tree.normals, tree.triangles, tree.leafTriangles, gameObject, 128, 20);
            DestroyImmediate(GetComponent<MeshCollider>());
        }
        mesh.colors = colors;
        GetComponent<MeshFilter>().mesh = mesh;

        return mesh;
    }

    public GameObject CreateBillboard(string path, string name)
    {
        GameObject camObject = Instantiate(Resources.Load("MtreeBillboardCamera") as GameObject); // create billboard and render it
        Camera cam = camObject.GetComponent<Camera>();
        Billboard bill = new Billboard(cam, gameObject, 512, 512);
        bill.SetupCamera();
        string texturePath = path + name + "_billboard.png";
        bill.Render(texturePath);
        DestroyImmediate(camObject);

        Mesh billboardMesh = bill.CreateMesh(); // create billboard mesh
        AssetDatabase.CreateAsset(billboardMesh, path + name + "_LOD4.mesh");

        GameObject billboard = new GameObject(name + "_LOD4"); // create billboard object and assign mesh
        MeshFilter meshFilter = billboard.AddComponent<MeshFilter>();
        meshFilter.mesh = billboardMesh;
        MeshRenderer meshRenderer = billboard.AddComponent<MeshRenderer>();

        Texture billboardTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)); // create material
        Material mat = bill.CreateMaterial(billboardTexture);
        AssetDatabase.CreateAsset(mat, path + name + "billboard.mat");
        meshRenderer.material = mat;

        return billboard;
    }

    public void BakeAo()
    {
        GenerateTree(ao:true);
    }

    public Mesh GenerateTree(bool ao=false)
    {
        tree = null;
        ExecuteFunctions();
        tree.Simplify(simplifyAngleThreshold[Lod], simplifyRadiusThreshold[Lod]);
        Mesh mesh = CreateMesh(ao);
        return mesh;
    }

    private void Start()
    {
        GenerateTree();
    }

    public void SaveAsPrefab()
    {
        string name = gameObject.name;
        string path = EditorUtility.SaveFilePanelInProject("Save Separate Mesh Asset", name, "", ""); // open save window
        if (string.IsNullOrEmpty(path))
            return;
        name = Path.GetFileName(path);
        path = Path.GetDirectoryName(path);
        if (AssetDatabase.LoadAssetAtPath(path + "/" + name + ".prefab", typeof(GameObject))) // Overriding prefab dialog
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "The prefab already exists. Do you want to overwrite it?", "Yes", "No"))
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(path, name + "_meshes"));
            }
            else
            {
                name += "_1";
            }
        }
    
        Mesh[] meshes = new Mesh[4];
        Debug.Log(path);
        string meshesFolder = AssetDatabase.CreateFolder(path, name + "_meshes");
        meshesFolder = AssetDatabase.GUIDToAssetPath(meshesFolder) + Path.DirectorySeparatorChar;
        Debug.Log(meshesFolder);
        //return;
        GameObject TreeObject = new GameObject(name); // Tree game object
        LODGroup group = TreeObject.AddComponent<LODGroup>(); // LOD Group
        int lodNumber = 4; // Creating LODs
        LOD[] lods = new LOD[lodNumber + 1];

        // Generating Billboard 
        Lod = 3;
        GenerateTree(true);
        GameObject billboard = CreateBillboard(meshesFolder, name);
        Renderer[] bill_re = new Renderer[1] { billboard.GetComponent<MeshRenderer>() };
        lods[lodNumber] = new LOD(.01f, bill_re);


        for (int lod= lodNumber-1; lod>-1; lod--) // create and save all LOD meshes
        {
            string meshPath = meshesFolder + name + "_LOD" + lod.ToString() + ".mesh"; //updating path for each LOD
            Lod = lod;
            Mesh mesh = GenerateTree(ao: true);
            meshes[lod] = mesh;
            AssetDatabase.CreateAsset(mesh, meshPath);
        }

        for (int i=0; i<lodNumber; i++) // assigning lod meshes to LOD array
        {
            GameObject go = new GameObject(name + "_LOD" + i.ToString());
            go.transform.parent = TreeObject.transform;
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = meshes[i];
            Renderer[] re =  new Renderer[1] { go.AddComponent<MeshRenderer>() }; // the renderer to put in LODs
            re[0].sharedMaterials = GetComponent<MeshRenderer>().sharedMaterials;
            float t = Mathf.Pow((i + 1)*1f / (lodNumber + 1), 1); // float between 0 and 1 following f(x) = pow(x, n)
            lods[i] = new LOD( (1-t)*0.9f + t*.01f, re); // assigning renderer
        }

        billboard.transform.parent = TreeObject.transform; // making billboard child of tree object
        

        group.SetLODs(lods); // assigning LODs to lod group
        group.RecalculateBounds();

        string prefabPath = path + "/" + name + ".prefab";
        Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
        PrefabUtility.ReplacePrefab(TreeObject, prefab, ReplacePrefabOptions.ReplaceNameBased);
        AssetDatabase.SaveAssets();
    }
}


