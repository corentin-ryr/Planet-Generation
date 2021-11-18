using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    //Public variables ===================================================
    [Range(1, 70)]
    public int nbSubdivision = 1;

    [Range(1f, 50f)]
    public float radius = 1f;

    // public Shader sphereShader;
    public Material material;
    public Shader shapeShader;
    public ComputeShader triangulationShader;

    //Mesh Data =======================================================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private int[] triangles;
    private Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = material;

        mesh = new Mesh();

        Isocahedron Sphere = new Isocahedron(nbSubdivision, radius, triangulationShader, UpdateMeshData);
        Sphere.CreateSphere();
    }

    // Update is called once per frame
    void Update()
    {
        // GenerateMesh();
    }

    void UpdateMeshData(Vector3Int[] trianglesData, Vector3[] verticesData)
    {
        vertices = verticesData;

        triangles = new int[trianglesData.Length * 3];
        for (int i = 0; i < trianglesData.Length; i++)
        {
            triangles[i * 3] = trianglesData[i].x;
            triangles[i * 3 + 1] = trianglesData[i].y;
            triangles[i * 3 + 2] = trianglesData[i].z;
        }

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        // Create the actual Unity mesh object
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        Debug.Log(triangles.Length / 3);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
