using System.Collections.Generic;
using UnityEngine;

public class PlanetIsocahedron : MonoBehaviour
{
    //Public variables ===================================================
    [Range(1, 70)]
    public int nbSubdivision = 1;

    [Min(1f)]
    public float radius = 1f;

    public float[] detailLevels;


    // Children ==============================
    public PlanetNodeIsocahedron planetNodePrefab;
    private List<PlanetNodeIsocahedron> nodes = new List<PlanetNodeIsocahedron>();

    // Ref to the player to compute distance =========
    public Player player;
    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        CreateNodes();
        previousPosition = player.transform.position;
    }

    void Update() {

        if ((previousPosition - player.transform.position).magnitude > 10)
        {
            RefreshLOD();
        }
    }

    private void RefreshLOD() {
        foreach (PlanetNodeIsocahedron node in nodes)
        {
            node.CheckAndSubdivide();
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
        node.radius = radius;
        node.player = player;
        node.detailLevels = detailLevels;

        float a = 2f;
        float h = Mathf.Sqrt(3f);
        float dMiddle = Mathf.Sqrt(3) * (1 + Metrics.PHI) / 3;
        node.initialVertices = new Vector3[] { new Vector3(-a / 2, -h / 3, dMiddle).normalized, new Vector3(0, 2 * h / 3, dMiddle).normalized, new Vector3(a / 2, -h / 3, dMiddle).normalized };
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

    private (Vector3[], Vector3Int[]) NodesPositions2()
    {
        List<Vector3> vertices = new List<Vector3>();
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f; //Golden ratio

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

        List<Vector3Int> faces = new List<Vector3Int>();
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
        

        return (vertices.ToArray(), faces.ToArray());

    }


}


