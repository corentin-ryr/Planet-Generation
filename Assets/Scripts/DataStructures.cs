using System.Collections.Generic;
using UnityEngine;
using Geometry;

public class NoDuplicatesList<T> : List<T>
{
    public NoDuplicatesList() : base() { }
    public NoDuplicatesList(T[] array) : base(array) { }

    public new void Add(T item)
    {
        if (!Contains(item))
            base.Add(item);
    }
}

public class FacesAndEdgesList : List<TriangleIndices>
{
    private NoDuplicatesList<Edge> edges;

    public FacesAndEdgesList() : base()
    {
        edges = new NoDuplicatesList<Edge>();
    }

    public new void Add(int v1, int v2, int v3)
    {
        if (!Contains(item))
            base.Add(item);
            edges.Add(new Edge(v1, v2));
            edges.Add(new Edge(v2, v3));
            edges.Add(new Edge(v3, v1));
    }
    public new void Add(TriangleIndices triangle) {
        this.Add(triangle.v1, triangle.v2, triangle.v3);
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