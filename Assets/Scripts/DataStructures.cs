using System.Collections.Generic;
using UnityEngine;
using Geometry;

public class NoDuplicatesList : List<Vector2Int>
{
    public NoDuplicatesList() : base() { }
    public NoDuplicatesList(Vector2Int[] array) : base(array) { }

    public new void Add(Vector2Int item)
    {
        if (!Contains(item) && !Contains(new Vector2Int(item.y, item.x)))
            base.Add(item);
    }
}

public class FacesAndEdgesList : List<Vector3Int>
{
    private NoDuplicatesList edges;

    public FacesAndEdgesList() : base()
    {
        edges = new NoDuplicatesList();
    }

    public new void Add(Vector3Int triangle)
    {

        if (!Contains(triangle))
        {
            base.Add(triangle);
            edges.Add(new Vector2Int(triangle.x, triangle.y));
            edges.Add(new Vector2Int(triangle.y, triangle.z));
            edges.Add(new Vector2Int(triangle.z, triangle.x));
        }
    }

    public new void Clear()
    {
        base.Clear();
        edges.Clear();
    }

    public NoDuplicatesList getEdges()
    {
        return edges;
    }


}

public class ExtendedDictionary<T, U> : Dictionary<T, U>
{
    public ExtendedDictionary() : base() { }
    public ExtendedDictionary(T[] keys, U[] values) : base()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (!this.ContainsKey(keys[i]))
            {
                this.Add(keys[i], values[i]);
            }
        }
    }

    public void getKeysAndValuesAsArray(out T[] keys, out U[] values)
    {
        List<T> keyList = new List<T>(this.Keys);
        keys = new T[keyList.Count];
        values = new U[keyList.Count];

        for (int i = 0; i < keyList.Count; i++)
        {
            keys[i] = keyList[i];
            values[i] = this[keyList[i]];
        }
    }
}