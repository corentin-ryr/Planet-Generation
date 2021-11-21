using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetChunk : MonoBehaviour
{
    // LOD information ===============
    private int LOD = 0;
    //Vertices of the lower level of detail (without terrain)
    private Vector3[] vertices;
    private int[] triangles;

    // Faces of the lower LOD (without terrain)

    // Mesh data =====================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    public Material material;

    private Dictionary<int, (int[], Vector3[])> meshDataLOD = new Dictionary<int, (int[], Vector3[])>();
    // private Dictionary<int, Vector3[]> verticesLOD = new Dictionary<int, Vector3[]>();




    public void SetInitialMeshData(FacesAndEdgesList faces, Vector3[] vertices)
    {
        meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = material;

        mesh = new Mesh();

        // this.faces = faces;
        this.vertices = vertices;


        this.triangles = new int[faces.Count * 3];
        for (int i = 0; i < faces.Count; i++)
        {
            triangles[i * 3] = faces[i].x;
            triangles[i * 3 + 1] = faces[i].y;
            triangles[i * 3 + 2] = faces[i].z;
        }

        CreateLOD(0, vertices, triangles);

        GenerateMesh();
    }

    // public void UpdateLOD(int LOD)
    // {
    //     this.LOD = LOD;

    //     if (!meshDataLOD.ContainsKey(LOD))
    //     {
    //         CreateLOD(LOD);
    //     }

    //     GenerateMesh();
    // }


    private void GenerateMesh()
    {
        // Create the actual Unity mesh object
        var meshData = meshDataLOD[LOD];

        mesh.vertices = meshData.Item2;
        mesh.triangles = meshData.Item1;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }


    private void CreateLOD(int LOD, Vector3[] vertices, int[] triangles)
    {
        meshDataLOD.Add(LOD, (triangles, vertices));
    }

}
