using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlanetNodeIsocahedron : MonoBehaviour
{
    // LOD information ===============
    public int segmentationLevel = 0;

    // 0: bottom left, 1: top, 2: bottom right
    public Vector3[] initialVertices;
    private int nbSubdivision = 10;
    //Distance of the center of the triangle to the center of the sphere.
    private float radius = 1f;
    private float dMiddle;


    //Set in the prefab ============
    public PlanetNodeIsocahedron planetNodePrefab;
    public ComputeShader triangulationShader;


    private PlanetNodeIsocahedron[] childrenNodes = new PlanetNodeIsocahedron[4];

    // Mesh data =====================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    public Material material;


    void Start()
    {
        meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = material;

        mesh = new Mesh();

        CheckAndSubdivide();
    }



    private void CheckAndSubdivide()
    {
        if (ShouldSubdivide())
        {
            //Create the nodes if they are not yet created and run their check and subdivide function
            RefreshMesh(false);
            InstantiateNode();
        }
        else
        {
            //Create and render the mesh for the chunk.
            Isocahedron Sphere = new Isocahedron(nbSubdivision, radius, triangulationShader, CreateMesh);
            Sphere.SubdivideFace(initialVertices);

        }
    }

    private bool ShouldSubdivide()
    {
        return segmentationLevel < 1;
    }

    private void CreateMesh(Vector3Int[] trianglesV3, Vector3[] vertices)
    {
        mesh.vertices = vertices;

        int[] triangles = new int[trianglesV3.Length * 3];
        for (int i = 0; i < trianglesV3.Length; i++)
        {
            triangles[i * 3] = trianglesV3[i].x;
            triangles[i * 3 + 1] = trianglesV3[i].y;
            triangles[i * 3 + 2] = trianglesV3[i].z;
        }
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        RefreshMesh(true);
    }


    private void RefreshMesh(bool showMesh)
    {
        if (showMesh)
        {
            // Create the actual Unity mesh object
            meshFilter.mesh = mesh;
        }
        else
        {
            meshFilter.mesh = null;
        }

    }


    private void InstantiateNode()
    {
        Vector3[] childrenVertices = getChildrenVertices();

        for (int i = 0; i < 4; i++)
        {
            PlanetNodeIsocahedron node = Instantiate(planetNodePrefab);
            node.transform.parent = gameObject.transform;
            node.planetNodePrefab = planetNodePrefab;
            node.segmentationLevel = segmentationLevel + 1;

            Quaternion upsideDown = i == 3 ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            Quaternion rotation = Quaternion.LookRotation((childrenVertices[i * 3] + childrenVertices[i * 3 + 1] + childrenVertices[i * 3 + 2] / 3)) * upsideDown;
            node.transform.localRotation = rotation;

            node.initialVertices = new Vector3[] {
                Quaternion.Inverse(rotation) * childrenVertices[i * 3],
                Quaternion.Inverse(rotation) * childrenVertices[i * 3 + 1],
                Quaternion.Inverse(rotation) * childrenVertices[i * 3 + 2]
            };


            childrenNodes[i] = node;
        }

    }


    public Vector3[] getChildrenVertices()
    {
        Vector3[] vertices = new Vector3[4 * 3];

        Vector3 a = (initialVertices[0] + initialVertices[1]) / 2;
        Vector3 b = (initialVertices[1] + initialVertices[2]) / 2;
        Vector3 c = (initialVertices[2] + initialVertices[0]) / 2;
        a = a * radius / a.magnitude;
        b = b * radius / b.magnitude;
        c = c * radius / c.magnitude;

        vertices[0] = initialVertices[0];
        vertices[1] = a;
        vertices[2] = c;

        vertices[3] = a;
        vertices[4] = initialVertices[1];
        vertices[5] = b;

        vertices[6] = c;
        vertices[7] = b;
        vertices[8] = initialVertices[2];

        vertices[9] = a;
        vertices[10] = b;
        vertices[11] = c;

        return vertices;

    }

}
