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
    public PlanetChunk PlanetChunkPrefab;

    //Mesh Data =======================================================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    private List<PlanetChunk> chunks = new List<PlanetChunk>();


    // Start is called before the first frame update
    void Start()
    {
        Isocahedron Sphere = new Isocahedron(nbSubdivision, radius, triangulationShader, CreateChunks);
        Sphere.CreateSphere();
    }


    void UpdateMeshData(Vector3Int[] trianglesData, Vector3[] verticesData)
    {
        int verticesPerChunk = nbSubdivision * nbSubdivision;
        vertices = verticesData;

        triangles = new int[trianglesData.Length * 3];
        for (int i = 0; i < trianglesData.Length; i++)
        {
            triangles[i * 3] = trianglesData[i].x;
            triangles[i * 3 + 1] = trianglesData[i].y;
            triangles[i * 3 + 2] = trianglesData[i].z;
        }

    }



    private void CreateChunks(Vector3Int[] trianglesData, Vector3[] verticesData)
    {
        int trianglesPerChunk = nbSubdivision * nbSubdivision;
        vertices = verticesData;

        PlanetChunk chunk;
        FacesAndEdgesList tempList = new FacesAndEdgesList();
        for (int i = 0; i < trianglesData.Length; i++)
        {
            tempList.Add(trianglesData[i]);

            if (i % trianglesPerChunk == trianglesPerChunk - 1)
            {
                chunk = Instantiate(PlanetChunkPrefab, gameObject.transform);
                chunk.transform.parent = gameObject.transform;
                chunk.material = material;
                chunk.SetInitialMeshData(tempList, verticesData);
                tempList.Clear();
            }

        }

    }
}
