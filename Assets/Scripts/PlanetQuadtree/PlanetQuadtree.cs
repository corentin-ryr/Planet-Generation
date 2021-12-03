using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetQuadtree : MonoBehaviour
{

    public PlanetNodeQuadtree planetNodePrefab;


    private List<PlanetNodeQuadtree> nodes = new List<PlanetNodeQuadtree>();

    // Start is called before the first frame update
    void Start()
    {
        CreateFaces();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateFaces()
    {
        for (int i = 0; i < 6; i++) //Create the six faces of the cube
        {
            PlanetNodeQuadtree node = Instantiate(planetNodePrefab, gameObject.transform);
            node.transform.parent = gameObject.transform;
            node.planetNodePrefab = planetNodePrefab;
            node.segmentationLevel = 0;
            nodes.Add(node);
        }
    }
}
