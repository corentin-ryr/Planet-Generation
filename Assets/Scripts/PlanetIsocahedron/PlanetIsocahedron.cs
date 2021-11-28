using System.Collections.Generic;
using UnityEngine;

public class PlanetIsocahedron : MonoBehaviour
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
    public PlanetNodeIsocahedron planetNodePrefab;

    //Mesh Data =======================================================
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    private List<PlanetNodeIsocahedron> nodes = new List<PlanetNodeIsocahedron>();


    // Start is called before the first frame update
    void Start()
    {
        // Isocahedron Sphere = new Isocahedron(nbSubdivision, radius, triangulationShader, CreateChunks);
        // Sphere.CreateSphere();

        CreateNodes();
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

    private void CreateNodes()
    {
        Vector3[] positions = NodesPositions();

        int i = 0;
        for (; i < 10; i++) //Create the six faces of the cube
        {
            InstantiateNode(Quaternion.LookRotation(positions[i]));
        }

        for (; i < 20; i++) //Create the six faces of the cube
        {
            InstantiateNode(Quaternion.LookRotation(positions[i]) * Quaternion.Euler(0, 0, 180));
        }
    }

    private void InstantiateNode(Quaternion quaternion)
    {
        PlanetNodeIsocahedron node = Instantiate(planetNodePrefab, gameObject.transform.position, quaternion);
        node.transform.parent = gameObject.transform;
        node.planetNodePrefab = planetNodePrefab;
        node.segmentationLevel = 0;

        float a = 2f;
        float h = Mathf.Sqrt(3f);
        float dMiddle = Mathf.Sqrt(3) * (1 + Metrics.PHI) / 3;
        node.initialVertices = new Vector3[] { new Vector3(-a / 2, -h / 3, dMiddle).normalized * radius, new Vector3(0, 2 * h / 3, dMiddle).normalized * radius, new Vector3(a / 2, -h / 3, dMiddle).normalized * radius };
        nodes.Add(node);
    }

    private Vector3[] NodesPositions()
    {
        float side_length = 1f; // 2 * Metrics.PHI * radius / (3  * Mathf.Sqrt(Metrics.PHI * Mathf.Sqrt(5)));
        // Value t1 is actually never used.
        float s = side_length;
        //double t1 = 2.0 * Mathf.PI / 5.0;
        float t2 = Mathf.PI / 10.0f;
        float t3 = 3.0f * Mathf.PI / 10.0f;
        float t4 = Mathf.PI / 5.0f;
        float d1 = s / 2.0f / Mathf.Sin(t4);
        float d2 = d1 * Mathf.Cos(t4);
        float d3 = d1 * Mathf.Cos(t2);
        float d4 = d1 * Mathf.Sin(t2);
        float Fx =
            (s * s - (2.0f * d3) * (2.0f * d3) -
                (d1 * d1 - d3 * d3 - d4 * d4)) /
                    (2.0f * (d4 - d1));
        float d5 = Mathf.Sqrt(0.5f *
            (s * s + (2.0f * d3) * (2.0f * d3) -
                (d1 - Fx) * (d1 - Fx) -
                    (d4 - Fx) * (d4 - Fx) - d3 * d3));
        float Fy = (Fx * Fx - d1 * d1 - d5 * d5) / (2.0f * d5);
        float Ay = d5 + Fy;

        Vector3 A = new Vector3(d1, Ay, 0);
        Vector3 B = new Vector3(d4, Ay, d3);
        Vector3 C = new Vector3(-d2, Ay, s / 2);
        Vector3 D = new Vector3(-d2, Ay, -s / 2);
        Vector3 E = new Vector3(d4, Ay, -d3);
        Vector3 F = new Vector3(Fx, Fy, 0);
        Vector3 G = new Vector3(Fx * Mathf.Sin(t2), Fy,
            Fx * Mathf.Cos(t2));
        Vector3 H = new Vector3(-Fx * Mathf.Sin(t3), Fy,
            Fx * Mathf.Cos(t3));
        Vector3 I = new Vector3(-Fx * Mathf.Sin(t3), Fy,
            -Fx * Mathf.Cos(t3));
        Vector3 J = new Vector3(Fx * Mathf.Sin(t2), Fy,
            -Fx * Mathf.Cos(t2));
        Vector3 K = new Vector3(Fx * Mathf.Sin(t3), -Fy,
            Fx * Mathf.Cos(t3));
        Vector3 L = new Vector3(-Fx * Mathf.Sin(t2), -Fy,
            Fx * Mathf.Cos(t2));
        Vector3 M = new Vector3(-Fx, -Fy, 0);
        Vector3 N = new Vector3(-Fx * Mathf.Sin(t2), -Fy,
            -Fx * Mathf.Cos(t2));
        Vector3 O = new Vector3(Fx * Mathf.Sin(t3), -Fy,
            -Fx * Mathf.Cos(t3));
        Vector3 P = new Vector3(d2, -Ay, s / 2);
        Vector3 Q = new Vector3(-d4, -Ay, d3);
        Vector3 R = new Vector3(-d1, -Ay, 0);
        Vector3 S = new Vector3(-d4, -Ay, -d3);
        Vector3 T = new Vector3(d2, -Ay, -s / 2);

        List<Vector3> points = new List<Vector3>();
        points.Add(A);
        points.Add(B);
        points.Add(C);
        points.Add(D);
        points.Add(E);
        
        points.Add(K);
        points.Add(L);
        points.Add(M);
        points.Add(N);
        points.Add(O);

        points.Add(F);
        points.Add(G);
        points.Add(H);
        points.Add(I);
        points.Add(J);

        points.Add(P);
        points.Add(Q);
        points.Add(R);
        points.Add(S);
        points.Add(T);

        return points.ToArray();

    }
}
