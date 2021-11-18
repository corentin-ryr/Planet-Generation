using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Crater
{
    public Vector3 center;
    public float radius;
}

public class PlanetShape : MonoBehaviour
{

    public ComputeShader computeShader;

    // Variable to store the craters data
    private Crater[] craters;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // #region Terrain generation
    // private void createTerrain()
    // {
    //     Vector3[] verticesArray = vertices.ToArray();

    //     // vertices = craterGeneration(verticesArray).ToList();

    // }

    // private Vector3[] craterGeneration(Vector3[] verticesArray)
    // {
    //     ComputeBuffer verticesBuffer = new ComputeBuffer(vertices.Count, sizeof(float) * 3);
    //     verticesBuffer.SetData(verticesArray);

    //     computeShader.SetBuffer(0, "vertices", verticesBuffer);
    //     computeShader.SetFloat("numVertices", vertices.Count);
    //     computeShader.SetFloat("testValue", testValue);

    //     computeShader.Dispatch(0, vertices.Count / 10, 1, 1);

    //     verticesBuffer.GetData(verticesArray);

    //     verticesBuffer.Dispose();

    //     return verticesArray;
    // }

    // #endregion
}
