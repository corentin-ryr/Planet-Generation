using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetNodeQuadtree : MonoBehaviour
{

    public PlanetNodeQuadtree planetNodePrefab { get; set; }
    public int segmentationLevel { get; set; }
    private List<PlanetNodeQuadtree> nodes = new List<PlanetNodeQuadtree>();

    // Start is called before the first frame update
    void Start()
    {

    }



    private void Subdivide()
    {
        if (segmentationLevel <= 3)
        {
            
        }
    }

}
