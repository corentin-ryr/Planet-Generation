﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Geometry;
using System.Linq;

public struct Crater
{
    public Vector3 center;
    public float radius;
}

public class Isocahedron : MonoBehaviour
{
    #region Variables
    //Parameters
    public float radius;
    public int[] subdivisionSequence = { 2, 3 };
    public float testValue = 1f;
    public bool GPU;

    public ComputeShader computeShader;
    public ComputeShader computeShaderSubdivideEdges;

    //Elements of the unity mesh
    private List<Vector3> vertices = new List<Vector3>();
    private FacesAndEdgesList faces = new FacesAndEdgesList();

    private Vector3Int[] triangles_array;
    private NoDuplicatesList edges;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Cache for the triangulation
    private ExtendedDictionary<int, int[]> middlePointIndexCache = new ExtendedDictionary<int, int[]>();

    // Variable to store the craters data
    private Crater[] craters;

    //Debug variables
    public int faceNumberDebug = 0;

    #endregion

    void Start()
    {
        createSphere();
    }

    public void createSphere()
    {

        edges = faces.getEdges();
        meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        createVertices(); // Creates only the points (vertices)
        triangulateVertices(); // Create the faces, aka the triangles between the points

        if (GPU)
        {
            refineSphereGPU(subdivisionSequence); // Subdivide every face by adding new points and triangulating them
        }
        else
        {
            for (int i = 0; i < subdivisionSequence.Length; i++)
            {
                refineSphere(subdivisionSequence[i]); // Subdivide every face by adding new points and triangulating them
                projectPoints(); // Multiply the magnitude of every points to have a bigger planet
            }
        }

        generateMesh(vertices, faces); // Pass the vertices and triangles to the mesh to render it
    }

    void Update()
    {
        // The sphere is not build, all we need to do is alter the height of the points
        List<Vector3> updatedVertices = createTerrain(); // Use a compute shader to move the heights of the vertices

        generateMesh(updatedVertices, faces); // Pass the vertices and triangles to the mesh to render it
    }

    #region Mesh generation

    private void generateMesh(List<Vector3> vertices, FacesAndEdgesList faces)
    {
        // Create the actual Unity mesh object
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        int[] triangles = new int[triangles_array.Length * 3];
        for (int i = 0; i < triangles_array.Length; i++)
        {
            triangles[i * 3] = triangles_array[i].x;
            triangles[i * 3 + 1] = triangles_array[i].y;
            triangles[i * 3 + 2] = triangles_array[i].z;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshRenderer.material.SetColor("_Color", Color.red);

        meshFilter.mesh = mesh;
    }

    private void createVertices()
    {
        // create 12 vertices of a icosahedron
        float t = (float)(1.0 + Math.Sqrt(5.0)) / 2.0f; //Golden ratio

        vertices.Add(new Vector3(-1, t, 0));
        vertices.Add(new Vector3(1, t, 0));
        vertices.Add(new Vector3(-1, -t, 0));
        vertices.Add(new Vector3(1, -t, 0));

        vertices.Add(new Vector3(0, -1, t));
        vertices.Add(new Vector3(0, 1, t));
        vertices.Add(new Vector3(0, -1, -t));
        vertices.Add(new Vector3(0, 1, -t));

        vertices.Add(new Vector3(t, 0, -1));
        vertices.Add(new Vector3(t, 0, 1));
        vertices.Add(new Vector3(-t, 0, -1));
        vertices.Add(new Vector3(-t, 0, 1));
    }

    private void triangulateVertices()
    {
        // create 20 triangles of the icosahedron

        // 5 faces around point 0
        faces.Add(new Vector3Int(0, 11, 5));
        faces.Add(new Vector3Int(0, 5, 1));
        faces.Add(new Vector3Int(0, 1, 7));
        faces.Add(new Vector3Int(0, 7, 10));
        faces.Add(new Vector3Int(0, 10, 11));

        // 5 adjacent faces
        faces.Add(new Vector3Int(1, 5, 9));
        faces.Add(new Vector3Int(5, 11, 4));
        faces.Add(new Vector3Int(11, 10, 2));
        faces.Add(new Vector3Int(10, 7, 6));
        faces.Add(new Vector3Int(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new Vector3Int(3, 9, 4));
        faces.Add(new Vector3Int(3, 4, 2));
        faces.Add(new Vector3Int(3, 2, 6));
        faces.Add(new Vector3Int(3, 6, 8));
        faces.Add(new Vector3Int(3, 8, 9));

        // 5 adjacent faces
        faces.Add(new Vector3Int(4, 9, 5));
        faces.Add(new Vector3Int(2, 4, 11));
        faces.Add(new Vector3Int(6, 2, 10));
        faces.Add(new Vector3Int(8, 6, 7));
        faces.Add(new Vector3Int(9, 8, 1));
    }


    private void refineSphereGPU(int[] subdivisionSequence, bool verbose = false)
    {
        int time0 = Environment.TickCount;

        int kernelSubdivide = computeShaderSubdivideEdges.FindKernel("SubdivideEdges");
        int kernelTriangulate = computeShaderSubdivideEdges.FindKernel("CreateFaces");

        // Compute buffer length =======================================================================
        int verticesBufferLength = vertices.Count;
        int lenghtKeys = edges.Count;
        int cacheLength = 1;
        foreach (int nbSubdivision in subdivisionSequence)
        {
            verticesBufferLength = verticesBufferLength + (nbSubdivision - 1) * edges.Count + faces.Count * (nbSubdivision - 2) * (nbSubdivision - 1) / 2;
            lenghtKeys = lenghtKeys + faces.Count * (nbSubdivision - 1);
            cacheLength = czcheLength + lenghtKeys * (nbSubdivision + 1);

        }

        // Send the buffers to the Compute Shader ======================================================
        Vector3[] verticesArray = vertices.ToArray();
        Array.Resize(ref verticesArray, verticesBufferLength);
        ComputeBuffer verticesBuffer = ComputeHelper.CreateAndSetBuffer(verticesArray, computeShaderSubdivideEdges, "vertices", kernelSubdivide);
        computeShaderSubdivideEdges.SetBuffer(kernelTriangulate, "vertices", verticesBuffer);

        ComputeBuffer edgesBuffer = ComputeHelper.CreateAndSetBuffer(edges.ToArray(), computeShaderSubdivideEdges, "edges", kernelSubdivide);

        int[] keysArray = new int[lenghtKeys];
        ComputeBuffer keysBuffer = ComputeHelper.CreateAndSetBuffer(keysArray, computeShaderSubdivideEdges, "keys", kernelSubdivide);
        computeShaderSubdivideEdges.SetBuffer(kernelTriangulate, "keys", keysBuffer);

        int[] cacheArray = new int[lenghtKeys * (nbSubdivision + 1)];
        ComputeBuffer cacheBuffer = ComputeHelper.CreateAndSetBuffer(cacheArray, computeShaderSubdivideEdges, "cache", kernelSubdivide);
        computeShaderSubdivideEdges.SetBuffer(kernelTriangulate, "cache", cacheBuffer);

        Vector3Int[] trianglesArray = new Vector3Int[faces.Count * nbSubdivision * nbSubdivision];
        for (int i = 0; i < trianglesArray.Length; i++)
        {
            if (i % (nbSubdivision * nbSubdivision) == 0) { trianglesArray[i] = faces[i / (nbSubdivision * nbSubdivision)]; }
            else { trianglesArray[i] = new Vector3Int(0, 0, 0); }
        }
        ComputeBuffer trianglesBuffer = ComputeHelper.CreateAndSetBuffer(trianglesArray, computeShaderSubdivideEdges, "triangles", kernelTriangulate);

        // Set the int and float values ==================================================================
        computeShaderSubdivideEdges.SetInt("nbVertices", vertices.Count);
        computeShaderSubdivideEdges.SetInt("nbEdges", edges.Count);
        computeShaderSubdivideEdges.SetInt("nbFaces", faces.Count);
        computeShaderSubdivideEdges.SetInt("nbSubdivision", nbSubdivision);
        computeShaderSubdivideEdges.SetFloat("radius", radius);

        // Launch the computation ========================================================================
        foreach (int nbSubdivision in subdivisionSequence)
        {
            ComputeHelper.Run(computeShaderSubdivideEdges, edges.Count, 1, 1, kernelSubdivide);
            ComputeHelper.Run(computeShaderSubdivideEdges, faces.Count, 1, 1, kernelTriangulate);

            trianglesBuffer.GetData(trianglesArray);

            faces.Clear();
            edges.Clear();
            foreach (Vector3Int item in trianglesArray) //It updates the edges at the same time
            {
                faces.Add(item);
            }

        }

        // Retrieve the data ============================================================================
        // trianglesBuffer.GetData(trianglesArray);
        // faces.Clear();
        // edges.Clear();

        triangles_array = trianglesArray;
        // foreach (Vector3Int item in trianglesArray) //It updates the edges at the same time
        // {
        //     faces.Add(item);
        // }

        verticesBuffer.GetData(verticesArray);
        vertices = verticesArray.ToList<Vector3>();

        if (verbose)
        {
            Debug.Log(faces[faceNumberDebug].x + "  " + faces[faceNumberDebug].y + "  " + faces[faceNumberDebug].z);
            Debug.Log(vertices.Count);
            Debug.Log(edges.Count);
            Debug.Log(faces.Count);
        }

        // Free the data on the GPU
        verticesBuffer.Dispose();
        edgesBuffer.Dispose();
        keysBuffer.Dispose();
        cacheBuffer.Dispose();
        trianglesBuffer.Dispose();


        Debug.Log("Time to execute the compute shader " + (int)(Environment.TickCount - time0));
    }

    #endregion

    #region CPU sphere generation

    private void refineSphere(int nbSubdivision)
    {
        int time0 = Environment.TickCount;
        middlePointIndexCache = new ExtendedDictionary<int, int[]>();

        // refine triangles
        var faces2 = new FacesAndEdgesList();
        foreach (var tri in faces) // We refine by adding nbSubdivision vertices to each side of each face
        {
            // Create the aditional vertices on the sides
            int[] a = subdivideEdge(tri.x, tri.y, nbSubdivision);
            int[] b = subdivideEdge(tri.y, tri.z, nbSubdivision);
            int[] c = subdivideEdge(tri.z, tri.x, nbSubdivision);

            faces2.Add(new Vector3Int(tri.z, c[1], b[nbSubdivision - 1])); // We create the triangle at the top

            for (int j = 1; j < nbSubdivision; j++) // At each step of the loop we have i * 2 + 1 triangles to create
            {
                int[] l1 = subdivideEdge(c[j], b[nbSubdivision - j], j);
                int[] l2 = subdivideEdge(c[j + 1], b[nbSubdivision - j - 1], j + 1);

                for (int k = 0; k < j + 1; k++) // Creating upside triangles
                {
                    faces2.Add(new Vector3Int(l1[k], l2[k], l2[k + 1]));
                }
                for (int k = 0; k < j; k++) // Creating downside triagles
                {
                    faces2.Add(new Vector3Int(l1[k], l2[k + 1], l1[k + 1]));
                }
            }

        }
        faces = faces2;

        Debug.Log("Time to execute the compute shader " + (int)(Environment.TickCount - time0));
    }


    private int[] subdivideEdge(int p1, int p2, int nbSubdivision)
    {
        // first check if we have it already
        int key1 = ((int)p1 << 16) + (int)p2;
        int key2 = ((int)p2 << 16) + (int)p1;

        int[] ret;
        if (this.middlePointIndexCache.TryGetValue(key1, out ret))
        {
            return ret;
        }
        if (this.middlePointIndexCache.TryGetValue(key2, out ret))
        {
            int[] reverse = Enumerable.Reverse(ret).ToArray();
            return reverse;
        }

        // not in cache, calculate it
        Vector3[] middleVertices = new Vector3[nbSubdivision - 1];

        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        for (int i = 0; i < nbSubdivision - 1; i++)
        {
            Vector3 middle = (point1 * (nbSubdivision - 1 - i) + point2 * (i + 1)) / nbSubdivision;

            middleVertices[i] = middle;
        }

        int[] indices = new int[nbSubdivision + 1];
        indices[0] = p1;
        for (int i = 1; i < nbSubdivision; i++)
        {
            vertices.Add(middleVertices[i - 1]);
            indices[i] = vertices.IndexOf(middleVertices[i - 1]);
        }
        indices[nbSubdivision] = p2;

        // store it, return index
        this.middlePointIndexCache.Add(key1, indices);

        return indices;
    }

    private int[] subdivideEdgeLight(int p1, int p2, int nbSubdivision)
    {
        // first check if we have it already
        int key1 = ((int)p1 << 16) + (int)p2;
        int key2 = ((int)p2 << 16) + (int)p1;

        int[] ret;
        if (this.middlePointIndexCache.TryGetValue(key1, out ret))
        {
            return ret;
        }
        if (this.middlePointIndexCache.TryGetValue(key2, out ret))
        {
            int[] reverse = Enumerable.Reverse(ret).ToArray();
            return reverse;
        }

        return null;
    }

    private void projectPoints() //Put the refined points back on the sphere
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i] / vertices[i].magnitude * radius;
        }
    }


    #endregion


    #region Terrain generation
    private List<Vector3> createTerrain()
    {
        Vector3[] verticesArray = vertices.ToArray();

        Vector3[] updatedVertices = craterGeneration(verticesArray);


        return new List<Vector3>(updatedVertices);
    }

    private Vector3[] craterGeneration(Vector3[] verticesArray)
    {
        ComputeBuffer verticesBuffer = new ComputeBuffer(vertices.Count, sizeof(float) * 3);
        verticesBuffer.SetData(verticesArray);

        computeShader.SetBuffer(0, "vertices", verticesBuffer);
        computeShader.SetFloat("numVertices", vertices.Count);
        computeShader.SetFloat("testValue", testValue);

        computeShader.Dispatch(0, vertices.Count / 10, 1, 1);

        verticesBuffer.GetData(verticesArray);

        verticesBuffer.Dispose();

        return verticesArray;
    }

    #endregion

    #region Helper functions

    private static T[][] Make2DArray<T>(T[] input, int height, int width)
    {
        // T[,] output = new T[height, width];
        List<T[]> output = new List<T[]>();
        for (int i = 0; i < height; i++)
        {
            T[] temp = new T[width];
            for (int j = 0; j < width; j++)
            {
                temp[j] = input[i * width + j];
            }
            output.Add(temp);
        }
        return output.ToArray();
    }

    #endregion


    #region Gizmos

    // void OnDrawGizmos()
    // {
    //     if (vertices.Count != 0)
    //     {
    //         Gizmos.color = Color.gray;

    //         foreach (var vertex in vertices)
    //         {
    //             Gizmos.DrawSphere(vertex, 0.05f);
    //         }

    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawSphere(vertices[faces[faceNumberDebug].x], 0.1f);
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(vertices[faces[faceNumberDebug].y], 0.1f);
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(vertices[faces[faceNumberDebug].z], 0.1f);

    //     }

    // }

    #endregion

}
