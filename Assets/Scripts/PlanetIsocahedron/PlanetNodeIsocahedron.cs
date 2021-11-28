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
            // Isocahedron Sphere = new Isocahedron(nbSubdivision, radius, triangulationShader, CreateMesh);
            // Sphere.SubdivideFace(initialVertices);

            mesh.vertices = initialVertices;
            mesh.triangles = new int[] { 2, 1, 0 };

            RefreshMesh(true);

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
        Vector3[] childrenPositions = getChildrenPosition();

        for (int i = 0; i < 4; i++)
        {
            Quaternion upsideDown = i == 3 ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            PlanetNodeIsocahedron node = Instantiate(planetNodePrefab, gameObject.transform.position, gameObject.transform.rotation * Quaternion.LookRotation(childrenPositions[i]) * upsideDown);
            node.transform.parent = gameObject.transform;
            node.planetNodePrefab = planetNodePrefab;
            node.segmentationLevel = segmentationLevel + 1;

            float a = 0.5f;
            float h = 0.5f;
            float dMiddle = childrenPositions[i].magnitude;
            node.initialVertices = new Vector3[] {
                new Vector3(-a / 2, -h / 3, dMiddle).normalized * radius,
                new Vector3(0, 2 * h / 3, dMiddle).normalized * radius,
                new Vector3(a / 2, -h / 3, dMiddle).normalized * radius
            };



            childrenNodes[i] = node;
        }

    }

    public Vector3[] getChildrenPosition()
    {
        Vector3[] positions = new Vector3[4 * 3];

        Vector3 a = (initialVertices[0] + initialVertices[1]) / 2;
        Vector3 b = (initialVertices[1] + initialVertices[2]) / 2;
        Vector3 c = (initialVertices[2] + initialVertices[0]) / 2;
        a = a * radius / a.magnitude;
        b = b * radius / b.magnitude;
        c = c * radius / c.magnitude;

        positions[0] = (initialVertices[0] + a + c) / 3;
        positions[1] = (a + initialVertices[1] + b) / 3;
        positions[2] = (b + initialVertices[2] + c) / 3;
        positions[3] = (a + b + c) / 3;

        return positions;
    }




}
