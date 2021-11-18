using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetChunk : MonoBehaviour
{
    // LOD information ===============
    private int LOD;
    //Vertices of the lower level of detail (without terrain)
    private List<Vector3> vertices = new List<Vector3>(); 
    // Faces of the lower LOD (without terrain)
    private FacesAndEdgesList faces = new FacesAndEdgesList();

    // Mesh data =====================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    private Dictionary<int, (int[], Vector3[])> meshDataLOD = new Dictionary<int, (int[], Vector3[])>();
    // private Dictionary<int, Vector3[]> verticesLOD = new Dictionary<int, Vector3[]>();


    public void UpdateLOD(int LOD)
    {
        this.LOD = LOD;

        if (!meshDataLOD.ContainsKey(LOD))
        {
            CreateLOD(LOD);
        }

        GenerateMesh();
    }


    private void GenerateMesh()
    {
        // Create the actual Unity mesh object
        var meshData = meshDataLOD[LOD];

        mesh.vertices = meshData.Item2;
        mesh.triangles = meshData.Item1;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }


    private void CreateLOD(int LOD)
    {

    }

}
